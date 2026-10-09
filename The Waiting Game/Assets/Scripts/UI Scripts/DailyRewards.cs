using NUnit.Framework;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DailyRewards.Daily;

public class DailyRewards : MonoBehaviour
{   
    [Serializable] 
    public struct Daily
    {
        public Image dailyImage; // Image representing the daily reward
        public bool isClaimed; 
        public Image checkmark; // Checkmark that appears when the reward is claimed
        public Image focusArrow; // Arrow that points to the next unclaimed reward

        [Tooltip("The reward for this daily. Can be money, a multiplier that lasts a certain duration, or an item.")]
        public int moneyReward;
        public float multiplierReward; // item that player can use to multiply their earnings for a limited time
        public int multiplierDuration; // duration of the multiplier in minutes
        public Item item; 
    }

    #region Variables
    [SerializeField] private Daily[] dailiesArray = new Daily[7]; // list of daily reward images
    [SerializeField] private TextMeshProUGUI timeLeft; // shows time left to claim next reward
    [SerializeField] private Button claimButton; // button to claim reward

    public MoneyManager moneyManager;
    public GameAssets gameAssets;

    public int dayIndex; // index of the current day in the dailiesArray

    public TextMeshProUGUI multiplierText; // string to display the duration of the multiplier in the UI



    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckDailyReset();
        UpdateDailies();

        if(moneyManager == null)
        {
            moneyManager = FindAnyObjectByType<MoneyManager>();
        }

        if(gameAssets == null)
        {
            gameAssets = FindAnyObjectByType<GameAssets>();
        }
    }

    private void Update()
    {
        UpdateMultText();
    }

    public void UpdateDailies()
    {
        //Get last claim time
        string lastTime = PlayerPrefs.GetString("LastClaimTime", DateTime.MinValue.ToString());

        DateTime lastClaimTime;

        if (!string.IsNullOrEmpty(lastTime))
        {
            lastClaimTime = DateTime.Parse(lastTime); // Parse the last claim time from PlayerPrefs
        }
        else
        {
            //player never clicked button before, so set lastClaimTime to a default value
            lastClaimTime = DateTime.MinValue;
        }

        foreach (var day in dailiesArray)
        {
            for (int i = 0; i < dailiesArray.Length; i++)
            {
                if (i < PlayerPrefs.GetInt("DaysClaimed", 0)) // Check if the day has been claimed
                {
                    dailiesArray[i].isClaimed = true; // Mark the day as claimed
                    dailiesArray[i].checkmark.enabled = true; // Show checkmark for claimed days
                    dailiesArray[i].dailyImage.color = Color.gray; // Change color of claimed days to gray

                    if (i == 6) // If all days have been claimed, reset the lastClaimTime to allow claiming again
                    {
                        lastClaimTime = DateTime.MinValue; // Reset lastClaimTime if all days have been claimed
                    }
                }
                else
                {
                    dailiesArray[i].checkmark.enabled = false; // Hide checkmark for unclaimed days
                    timeLeft.text = ""; // Clear the time left text since no days have been claimed
                }

                dailiesArray[i].focusArrow.enabled = (i == PlayerPrefs.GetInt("DaysClaimed", 0)); // Show focus arrow for the next unclaimed day
                dayIndex = PlayerPrefs.GetInt("DaysClaimed", 0);
                PlayerPrefs.SetInt("DaysClaimed", 0);
                PlayerPrefs.Save();
            }
        }

        //Enable/disable claim button

        if (lastClaimTime == DateTime.MinValue ||
            DateTime.Now - lastClaimTime >= TimeSpan.FromHours(24))
        {
            claimButton.interactable = true;
        }
        else
        {
            claimButton.interactable = false;
            timeLeft.text = GetTimeToNextClaim();
        }
    }



    private void CheckDailyReset()
    {
        string lastTime = PlayerPrefs.GetString("LastClaimTime", "");

        // If the player has never claimed a reward, don't reset anything.
        if (string.IsNullOrEmpty(lastTime))
            return;

        if (!DateTime.TryParse(lastTime, out DateTime lastClaimTime))
            return;

        // Reset if 24 hours have passed since the last claim.
        if (DateTime.Now - lastClaimTime >= TimeSpan.FromHours(24))
        {
            PlayerPrefs.SetInt("DaysClaimed", 0);

            // Clear the last claim time so the player can claim Day 1.
            PlayerPrefs.DeleteKey("LastClaimTime");

            PlayerPrefs.Save();

            Debug.Log("24 hours passed without a claim. Daily rewards reset.");
        }
    }

    private string GetTimeToNextClaim()
    {
        //EVERY DAY AT MIDNIGHT
        int hours = Mathf.FloorToInt((float)(DateTime.Today.AddDays(1) - DateTime.Now).TotalHours);
        int minutes = Mathf.FloorToInt((float)(DateTime.Today.AddDays(1) - DateTime.Now).TotalMinutes) % 60;
        return (hours + " hours and " + minutes + " minutes left to claim next reward.");

        /*//EVERY 24 HOURS
        string lastTime = PlayerPrefs.GetString("LastClaimTime", "");

        if (!DateTime.TryParse(lastTime, out DateTime lastClaimTime))
            return "";

        TimeSpan remaining =
            lastClaimTime.AddHours(24) - DateTime.Now;

        if (remaining < TimeSpan.Zero)
            remaining = TimeSpan.Zero;

        int hours = (int)remaining.TotalHours;
        int minutes = remaining.Minutes;

        return $"{hours} hours and {minutes} minutes left to claim next reward.";*/
    }


    public void MarkDailyClaimed()
    {
        // Logic to claim the gift and update the UI
        dailiesArray[PlayerPrefs.GetInt("DaysClaimed", 0)].dailyImage.color = Color.gray; // Change color of the claimed day to gray
        dailiesArray[PlayerPrefs.GetInt("DaysClaimed", 0)].isClaimed = true; // Mark the day as claimed
    }

    public void ClaimGift()
    {
        PlayerPrefs.SetString("LastClaimTime", DateTime.Now.ToString()); // Save the current time as the last claim time
        PlayerPrefs.SetInt("DaysClaimed", PlayerPrefs.GetInt("DaysClaimed", 0) + 1); // Increment the number of days claimed
        claimButton.interactable = false;
        timeLeft.text = GetTimeToNextClaim();

        if(dailiesArray[PlayerPrefs.GetInt("DaysClaimed", 0) - 1].moneyReward > 0)
        {
            // Give the player their money reward
            moneyManager.money += dailiesArray[PlayerPrefs.GetInt("DaysClaimed", 0) - 1].moneyReward; // Add the money reward to the player's total money
        }
        
        if (dailiesArray[dayIndex].multiplierReward > 0)
        {
            // Don't allow multiple temporary buffs to overlap.
            if (multiplierCoroutine != null)
            {
                StopCoroutine(multiplierCoroutine);

                // Restore the original multipliers before applying the new buff.
                moneyManager.wageMultiplier = PlayerPrefs.GetFloat("WageMultiplier", 0.1f);
                moneyManager.inputMoneyMultiplier = PlayerPrefs.GetFloat("InputMultiplier", 0.1f);

                multiplierCoroutine = null;
            }

            float multiplier =
                dailiesArray[dayIndex].multiplierReward;

            float duration =
                dailiesArray[dayIndex].multiplierDuration;

            moneyManager.ActivateTemporaryBuff(
                multiplier,
                duration
            );

            multiplierCoroutine = StartCoroutine(
                ActivateMultiplier(multiplier, duration * 60)
            );
        }
        
        if(dailiesArray[PlayerPrefs.GetInt("DaysClaimed", 0) - 1].item != null)
        {
            dailiesArray[PlayerPrefs.GetInt("DaysClaimed", 0) - 1].item = dailiesArray[PlayerPrefs.GetInt("DaysClaimed", 0) - 1].item; // Get the item reward for the claimed day
            dailiesArray[PlayerPrefs.GetInt("DaysClaimed", 0) - 1].item.isPurchased = true; // Mark the item as purchased 

            gameAssets.shopItems.Add(dailiesArray[PlayerPrefs.GetInt("DaysClaimed", 0) - 1].item); // Add the item to the shop already purchased

            //Claimed items do not save to the gameassets list right now. Either add that here or hide it and unhide it from the shop here

            Debug.Log($"Item {dailiesArray[PlayerPrefs.GetInt("DaysClaimed", 0) - 1].item.name} added to shop.");
        }


        UpdateDailies();
        moneyManager.SaveCurrency();
    }


    public void UpdateMultText()
    {
        if (moneyManager == null || multiplierText == null)
            return;

        if (!moneyManager.IsTemporaryBuffActive)
        {
            multiplierText.text = "";
            return;
        }

        float multiplier = moneyManager.ActiveBuffMultiplier;
        float remainingMinutes = moneyManager.RemainingBuffMinutes;

        int minutes = Mathf.FloorToInt(remainingMinutes);
        int seconds = Mathf.FloorToInt(
            (remainingMinutes - minutes) * 60f
        );

        multiplierText.text = $"{multiplier}X $ for {minutes:D2}:{seconds:D2}";
    }

    private Coroutine multiplierCoroutine;

    private IEnumerator ActivateMultiplier(float multiplier, float duration)
    {
        // Store the current permanent multiplier values.
        float originalWageMultiplier = moneyManager.wageMultiplier;
        float originalInputMultiplier = moneyManager.inputMoneyMultiplier;

        // Apply the temporary multiplier.
        moneyManager.wageMultiplier *= multiplier;
        moneyManager.inputMoneyMultiplier *= multiplier;

        Debug.Log($"Multiplier x{multiplier} active for {duration} seconds.");

        // Keep the buff active for the specified duration.
        yield return new WaitForSecondsRealtime(duration);

        // Restore the original multipliers.
        moneyManager.wageMultiplier = originalWageMultiplier;
        moneyManager.inputMoneyMultiplier = originalInputMultiplier;

        moneyManager.UpdateWageMultText();
        moneyManager.UpdateInputWageText();

        Debug.Log("Temporary multiplier expired.");

        multiplierCoroutine = null;
    }
}

