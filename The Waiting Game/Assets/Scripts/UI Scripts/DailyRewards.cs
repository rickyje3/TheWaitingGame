using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using NUnit.Framework;

public class DailyRewards : MonoBehaviour
{   
    [Serializable] 
    public struct Daily
    {
        public Image dailyImage; // Image representing the daily reward
        public bool isClaimed; 
        public Image checkmark; // Checkmark that appears when the reward is claimed
        public Image focusArrow; // Arrow that points to the next unclaimed reward
        // Add type of reward here, e.g. coins, temporary buffs, exclusive items, etc.
    }

    #region Variables

    [SerializeField] private Daily[] dailiesArray = new Daily[7]; // list of daily reward images
    [SerializeField] private TextMeshProUGUI timeLeft; // shows time left to claim next reward
    [SerializeField] private Button claimButton; // button to claim reward

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateDailies();
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
                }
                else
                {
                    dailiesArray[i].checkmark.enabled = false; // Hide checkmark for unclaimed days
                    timeLeft.text = ""; // Clear the time left text since no days have been claimed
                }

                dailiesArray[i].focusArrow.enabled = (i == PlayerPrefs.GetInt("DaysClaimed", 0)); // Show focus arrow for the next unclaimed day
            }
        }

        //Enable/disable claim button

        if (DateTime.Today > lastClaimTime)
        {
            claimButton.interactable = true;
        }
        else
        {
            claimButton.interactable = false;
            timeLeft.text = GetTimeToNextClaim();
        }
    }

    private string GetTimeToNextClaim()
    {
        int hours = Mathf.FloorToInt((float)(DateTime.Today.AddDays(1) - DateTime.Now).TotalHours);
        int minutes = Mathf.FloorToInt((float)(DateTime.Today.AddDays(1) - DateTime.Now).TotalMinutes) % 60;
        return (hours + " hours and " + minutes + " minutes left to claim next reward.");
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

        // Logic to give the player their reward goes here

        UpdateDailies();
    }
}

