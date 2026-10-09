using System;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public DesktopActivityManager activityManager;
    /*[HideInInspector]*/ public float money;
    public float wageMultiplier; // Multiplier that determines player wage
    public float inputMoneyMultiplier; // Multiplier that determines money per input

    private float wageMultCost;
    //private float inputMoneyMultCost;

    public TextMeshProUGUI wageMultMenuText;
    public TextMeshProUGUI inputMultMenuText;

    public TextMeshProUGUI wageMultMenuCostText;
    public TextMeshProUGUI inputMultMenuCostText;

    public TextMeshProUGUI moneyText;
    public MainMenu mainMenu;

    public SoundFeedback soundFeedback;


    [Header("Temporary Multiplier Buff")]
    public float temporaryMultiplier = 1f;

    private long buffExpirationUnix; // Unix timestamp for when the temporary multiplier buff expires

    private const string BuffMultiplierKey = "DailyBuffMultiplier";
    private const string BuffExpirationKey = "DailyBuffExpiration";

    public bool IsTemporaryBuffActive => // Check if the temporary multiplier buff is active
        temporaryMultiplier > 1f &&
        DateTimeOffset.UtcNow.ToUnixTimeSeconds() < buffExpirationUnix;

    public float EffectiveWageMultiplier => // Calculate the effective wage multiplier, considering any active temporary multiplier buff
        wageMultiplier * (IsTemporaryBuffActive ? temporaryMultiplier : 1f);

    public float EffectiveInputMoneyMultiplier => // Calculate the effective input money multiplier, considering any active temporary multiplier buff
        inputMoneyMultiplier * (IsTemporaryBuffActive ? temporaryMultiplier : 1f);

    public float ActiveBuffMultiplier =>
        IsTemporaryBuffActive ? temporaryMultiplier : 1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(activityManager == null)
        activityManager = GetComponent<DesktopActivityManager>();


        money = PlayerPrefs.GetFloat("Money", 0f); // Load saved money or start at 0 if not found
        wageMultiplier = PlayerPrefs.GetFloat("WageMultiplier", .1f); // Load saved wage multiplier or start at 1 if not found
        inputMoneyMultiplier = PlayerPrefs.GetFloat("InputMoneyMultiplier", .1f); // Load saved wage multiplier or start at 1 if not found

        wageMultCost = PlayerPrefs.GetFloat("WageMultCost", 10f);
        //inputMoneyMultCost = PlayerPrefs.GetFloat("InputMoneyMultCost", 10f);

        Debug.Log("Loaded money: " + money);
        Debug.Log("Loaded wage mult: " + wageMultiplier);
        Debug.Log("Loaded input money mult: " + inputMoneyMultiplier);
        UpdateMoneyText();
        UpdateWageMultText();
        UpdateInputWageText();
    }

    void FixedUpdate()
    {
        //Check if working
        if (activityManager.CurrentActivity == DesktopActivityManager.ActivityType.Working && !mainMenu.isShopOpen && !mainMenu.isMenuOpen)
        {
            money += Time.fixedDeltaTime * EffectiveWageMultiplier; // Increase money based on time spent working
            activityManager.workTimer += Time.deltaTime;
            UpdateMoneyText();
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 1)
        {
            money += EffectiveInputMoneyMultiplier;
            UpdateMoneyText();
        }

        // Expire the buff even if the game has been running for a while.
        if (temporaryMultiplier > 1f &&
            DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= buffExpirationUnix)
        {
            ClearTemporaryBuff();
        }

        if (Input.anyKeyDown &&
            !Input.GetKeyDown(KeyCode.Escape) &&
            Time.timeScale == 1)
        {
            money += EffectiveInputMoneyMultiplier;
            UpdateMoneyText();
        }
    }

    public void UpdateMoneyText()
    {
        moneyText.text = "$" + money.ToString("F2"); // update the money text up to 2 decimal places
    }

    public void UpdateWageMultText()
    {
        wageMultMenuText.text = "$" + (wageMultiplier * 60).ToString("F2") + "/Minute"; // update the wage multiplier text up to 2 decimal places
        wageMultMenuCostText.text = "$" + wageMultCost.ToString("F2"); // update the wage multiplier cost text up to 2 decimal places
    }

    public void UpdateInputWageText()
    {
        inputMultMenuText.text = "$" + inputMoneyMultiplier.ToString("F2"); // update the wage multiplier text up to 2 decimal places
        //inputMultMenuCostText.text = "$" + inputMoneyMultCost.ToString("F2"); // update the wage multiplier cost text up to 2 decimal places
    }

    public void UpgradeWageMult()
    {
        if(money < wageMultCost)
        {
            Debug.Log("Not enough money to upgrade wage multiplier.");
            soundFeedback.PlaySound(SoundType.WrongPlacement);
            return;
        }
        else
        {
            money -= wageMultCost; // Deduct the cost from the player's money
            soundFeedback.PlaySound(SoundType.Purchase);
            UpdateMoneyText();

            float newWageMult = wageMultiplier * 1.1f; // Increase wage multiplier by 10%
            wageMultiplier = newWageMult;

            float newWageMultCost = wageMultCost * 1.15f; // Increase cost by 15%
            wageMultCost = newWageMultCost;
            SaveCurrency();
            UpdateWageMultText();
        }
    }

    public void UpgradeInputMult()
    {
         float newInputMult = inputMoneyMultiplier * 1.1f; // Increase input money multiplier by 10%
         inputMoneyMultiplier = newInputMult;

         //float newInputMultCost = inputMoneyMultCost * 1.12f; // Increase cost by 12%
         //inputMoneyMultCost = newInputMultCost;
         SaveCurrency();
         UpdateInputWageText();       
    }


    public void SaveCurrency()
    {
        PlayerPrefs.SetFloat("Money", money);
        Debug.Log("Currency saved: " + money);
        PlayerPrefs.SetFloat("WageMultiplier", wageMultiplier);
        PlayerPrefs.SetFloat("InputMoneyMultiplier", inputMoneyMultiplier);
        PlayerPrefs.SetFloat("WageMultCost", wageMultCost);
        //PlayerPrefs.SetFloat("InputMoneyMultCost", inputMoneyMultCost);
        PlayerPrefs.Save();
    }



    public void ActivateTemporaryBuff(float multiplier, float durationMinutes)
    {
        // Start or replace the current temporary buff.
        temporaryMultiplier = multiplier;

        buffExpirationUnix = DateTimeOffset.UtcNow
            .AddMinutes(durationMinutes)
            .ToUnixTimeSeconds();

        PlayerPrefs.SetFloat(BuffMultiplierKey, temporaryMultiplier);
        PlayerPrefs.SetString(
            BuffExpirationKey,
            buffExpirationUnix.ToString()
        );

        PlayerPrefs.Save();

        UpdateWageMultText();
        UpdateInputWageText();

        Debug.Log(
            $"Temporary x{multiplier} buff activated for " +
            $"{durationMinutes} minutes."
        );
    }

    private void LoadTemporaryBuff()
    {
        temporaryMultiplier = PlayerPrefs.GetFloat(BuffMultiplierKey, 1f); // Load the multiplier from PlayerPrefs, defaulting to 1 if not found

        string savedExpiration = PlayerPrefs.GetString( // Load the expiration time from PlayerPrefs, defaulting to "0" if not found
            BuffExpirationKey, "0"
        );

        if (!long.TryParse(savedExpiration, out buffExpirationUnix)) // If parsing fails, default to 0 (no buff).
            buffExpirationUnix = 0;

        // If the buff expired while the game was closed, clear it.
        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= buffExpirationUnix) // If the buff has expired
        {
            ClearTemporaryBuff();
        }
    }

    private void ClearTemporaryBuff()
    {
        temporaryMultiplier = 1f;
        buffExpirationUnix = 0;

        PlayerPrefs.DeleteKey(BuffMultiplierKey);
        PlayerPrefs.DeleteKey(BuffExpirationKey);
        PlayerPrefs.Save();

        UpdateWageMultText();
        UpdateInputWageText();

        Debug.Log("Temporary multiplier buff expired.");
    }

    public float RemainingBuffMinutes
    {
        get
        {
            if (!IsTemporaryBuffActive)
                return 0f;

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            return Mathf.Max(
                0f,
                (buffExpirationUnix - now) / 60f
            );
        }
    }
}
