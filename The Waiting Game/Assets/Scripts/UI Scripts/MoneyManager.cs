using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public DesktopActivityManager activityManager;
    /*[HideInInspector]*/ public float money;
    public float wageMultiplier = 1; // Multiplier that determines player wage
    public float inputMoneyMultiplier = 1; // Multiplier that determines money per input

    private float wageMultCost;
    private float inputMoneyMultCost;

    public TextMeshProUGUI wageMultMenuText;
    public TextMeshProUGUI inputMultMenuText;

    public TextMeshProUGUI wageMultMenuCostText;
    public TextMeshProUGUI inputMultMenuCostText;

    public TextMeshProUGUI moneyText;
    public MainMenu mainMenu;

    public SoundFeedback soundFeedback;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(activityManager == null)
        activityManager = GetComponent<DesktopActivityManager>();


        money = PlayerPrefs.GetFloat("Money", 0f); // Load saved money or start at 0 if not found
        wageMultiplier = PlayerPrefs.GetFloat("WageMultiplier", 1f); // Load saved wage multiplier or start at 1 if not found
        inputMoneyMultiplier = PlayerPrefs.GetFloat("InputMoneyMultiplier", 1f); // Load saved wage multiplier or start at 1 if not found

        wageMultCost = PlayerPrefs.GetFloat("WageMultCost", 10f);
        inputMoneyMultCost = PlayerPrefs.GetFloat("InputMoneyMultCost", 10f);

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
            money += Time.fixedDeltaTime * wageMultiplier; // Increase money based on time spent working
            activityManager.workTimer += Time.deltaTime;
            UpdateMoneyText();
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 1)
        {
            money += inputMoneyMultiplier;
            UpdateMoneyText();
        }
    }

    public void UpdateMoneyText()
    {
        moneyText.text = "$" + money.ToString("F2"); // update the money text up to 2 decimal places
    }

    public void UpdateWageMultText()
    {
        wageMultMenuText.text = "$" + wageMultiplier.ToString("F2") + "/Second"; // update the wage multiplier text up to 2 decimal places
        wageMultMenuCostText.text = "$" + wageMultCost.ToString("F2"); // update the wage multiplier cost text up to 2 decimal places
    }

    public void UpdateInputWageText()
    {
        inputMultMenuText.text = "$" + inputMoneyMultiplier.ToString("F2"); // update the wage multiplier text up to 2 decimal places
        inputMultMenuCostText.text = "$" + inputMoneyMultCost.ToString("F2"); // update the wage multiplier cost text up to 2 decimal places
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

            float newWageMult = wageMultiplier * 1.10f; // Increase wage multiplier by 10%
            wageMultiplier = newWageMult;

            float newWageMultCost = wageMultCost * 1.15f; // Increase cost by 15%
            wageMultCost = newWageMultCost;
            SaveCurrency();
            UpdateWageMultText();
        }
    }

    public void UpgradeInputMult()
    {
        if(money < inputMoneyMultCost)
        {
            Debug.Log("Not enough money to upgrade input money multiplier.");
            soundFeedback.PlaySound(SoundType.WrongPlacement);
            return;
        }
        else
        {
            money -= inputMoneyMultCost; // Deduct the cost from the player's money
            soundFeedback.PlaySound(SoundType.Purchase);
            UpdateMoneyText();

            float newInputMult = inputMoneyMultiplier * 1.02f; // Increase input money multiplier by 2%
            inputMoneyMultiplier = newInputMult;

            float newInputMultCost = inputMoneyMultCost * 1.15f; // Increase cost by 15%
            inputMoneyMultCost = newInputMultCost;
            SaveCurrency();
            UpdateInputWageText();
        }
    }


    public void SaveCurrency()
    {
        PlayerPrefs.SetFloat("Money", money);
        Debug.Log("Currency saved: " + money);
        PlayerPrefs.SetFloat("WageMultiplier", wageMultiplier);
        PlayerPrefs.SetFloat("InputMultiplier", inputMoneyMultiplier);
        PlayerPrefs.SetFloat("WageMultCost", wageMultCost);
        PlayerPrefs.SetFloat("InputMoneyMultCost", inputMoneyMultCost);
        PlayerPrefs.Save();
    }
}
