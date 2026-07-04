using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public DesktopActivityManager activityManager;
    /*[HideInInspector]*/ public float money;
    private float wageMultiplier; // Multiplier that determines player wage
    public TextMeshProUGUI moneyText; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(activityManager == null)
        activityManager = GetComponent<DesktopActivityManager>();


        money = PlayerPrefs.GetFloat("Money", 0f); // Load saved money or start at 0 if not found
        wageMultiplier = PlayerPrefs.GetFloat("WageMultiplier", 0.1f); // Load saved wage multiplier or start at 0.1 if not found
        Debug.Log("Loaded money: " + money);
        Debug.Log("Loaded wage mult: " + wageMultiplier);
        UpdateMoneyText();
    }

    void FixedUpdate()
    {
        //Check if working
        if (activityManager.CurrentActivity == DesktopActivityManager.ActivityType.Working)
        {
            money += Time.fixedUnscaledDeltaTime * wageMultiplier; // Increase money based on time spent working
            activityManager.workTimer += Time.deltaTime;
            UpdateMoneyText();
        }
    }

    public void UpdateMoneyText()
    {
        moneyText.text = "$ " + money.ToString("F2"); // update the money text up to 2 decimal places
    }

    public void SaveCurrency()
    {
        PlayerPrefs.SetFloat("Money", money);
        Debug.Log("Currency saved: " + money);
        PlayerPrefs.SetFloat("WageMultiplier", wageMultiplier);
        Debug.Log("Currency saved: " + money);
        PlayerPrefs.Save();
    }
}
