using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class DesktopActivityManager : MonoBehaviour
{
    public TextMeshProUGUI activityDebugText;

    // -----------------------------
    // WINDOWS API IMPORTS
    // -----------------------------

    // Gets the currently focused window
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    // Gets the process ID attached to a window
    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(
        IntPtr hWnd,
        out uint processId);

    // Window title detection
    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetWindowText(
    IntPtr hWnd,
    System.Text.StringBuilder text,
    int count);


    // ACTIVITY TYPES
    public enum ActivityType
    {
        Working,
        Gaming,
        Watching,
        Browsing,
        Idle,
        Gooning,
        Unknown
    }

    // -----------------------------
    // TREND DATA
    // -----------------------------

    [Serializable]
    public class ActivityTrend
    {
        // Total seconds spent in this activity
        public float TotalTime;

        // Number of times this activity occurred
        public int Sessions;

        // Tracks which hours of the day
        // this activity commonly happens
        // Example:
        // HourCounts[21] = activity happened at 9 PM
        public int[] HourCounts = new int[24];
    }

    // -----------------------------
    // CURRENT STATE
    // -----------------------------

    // The player's current detected activity
    public ActivityType CurrentActivity;

    // Stores trend data for each activity type
    public Dictionary<ActivityType, ActivityTrend> Trends =
        new Dictionary<ActivityType, ActivityTrend>();

    // Timer for how often detection happens
    float checkTimer;

    // Tracks how long the current activity
    // has been active
    float activityTimer;

    // -----------------------------
    // START
    // -----------------------------

    void Start()
    {
        // Create trend data for every activity type
        foreach (ActivityType type in Enum.GetValues(typeof(ActivityType)))
        {
            Trends[type] = new ActivityTrend();
        }
    }

    // -----------------------------
    // UPDATE LOOP
    // -----------------------------

    void Update()
    {
        // Increase timers every frame
        checkTimer += Time.deltaTime;
        activityTimer += Time.deltaTime;

        // Only check activity every 5 seconds
        // so we're not constantly polling Windows
        if (checkTimer >= 10f)
        {
            checkTimer = 0f;

            // Detect what the player is currently doing
            ActivityType newActivity = DetectActivity();

            // If activity changed...
            if (newActivity != CurrentActivity)
            {
                // Save data from previous activity
                SaveActivityData(CurrentActivity, activityTimer);

                // Reset timer for new activity
                activityTimer = 0f;

                // Update current activity
                CurrentActivity = newActivity;

                UnityEngine.Debug.Log("New Activity: " + CurrentActivity);

                if (activityDebugText != null)
                {
                    activityDebugText.text = CurrentActivity.ToString();
                }
                else
                {
                    UnityEngine.Debug.LogWarning("activityDebugText is not assigned in the Inspector.");
                }
            }
        }
    }

    // DETECT CURRENT PLAYER ACTIVITY

    ActivityType DetectActivity()
    {
        // Get the currently focused window
        IntPtr hwnd = GetForegroundWindow();

        // Get process ID from that window
        GetWindowThreadProcessId(hwnd, out uint pid);

        System.Text.StringBuilder buffer =
        new System.Text.StringBuilder(256);

        GetWindowText(hwnd, buffer, 256);

        string windowTitle = buffer.ToString().ToLower();

        UnityEngine.Debug.Log(windowTitle);

        try
        {
            // Get process info
            Process process = Process.GetProcessById((int)pid);

            // Convert process name to lowercase
            string processName = process.ProcessName.ToLower();



            // Ignore the companion itself
            if (processName.Contains(Application.productName.ToLower()))
            {
                return CurrentActivity;
            }

            UnityEngine.Debug.Log("Detected Process: " + processName);

            if (processName.Contains("unity") ||
                processName.Contains("code") ||
                processName.Contains("visual studio") ||
                processName.Contains("sln") ||
                processName.Contains("code") ||
                processName.Contains("github") ||
                processName.Contains("photoshop") ||
                processName.Contains("unreal") ||
                processName.Contains("maya") ||
                processName.Contains("adobe") ||
                processName.Contains("word") ||
                processName.Contains("excel") ||
                processName.Contains("powerpoint") ||
                processName.Contains("zoom") ||
                processName.Contains("teams") ||
                processName.Contains("blender"))
            {
                return ActivityType.Working;
            }

            if (processName.Contains("steam") ||
                processName.Contains("minecraft"))
            {
                return ActivityType.Gaming;
            }

            if (processName.Contains("chrome") ||
                processName.Contains("firefox") ||
                processName.Contains("edge") ||
                processName.Contains("brave") ||
                processName.Contains("opera"))
            {
                // Watching tabs
                if (windowTitle.Contains("youtube") ||
                    windowTitle.Contains("netflix") ||
                    windowTitle.Contains("istreameast") ||
                    windowTitle.Contains("fmovies") ||
                    windowTitle.Contains("twitch"))
                {
                    return ActivityType.Watching;
                }

                // Work-related tabs
                if (windowTitle.Contains("docs") ||
                    windowTitle.Contains("sheets") ||
                    windowTitle.Contains("trello") ||
                    windowTitle.Contains("clickup") ||
                    windowTitle.Contains("microsoft") ||
                    windowTitle.Contains("github") ||
                    windowTitle.Contains("notion"))
                {
                    return ActivityType.Working;
                }

                if (windowTitle.Contains("xvideos") ||
                    windowTitle.Contains("pornhub") ||
                    windowTitle.Contains("redtube") ||
                    windowTitle.Contains("youporn") ||
                    windowTitle.Contains("rule34"))
                {
                    return ActivityType.Gooning;
                }

                // Generic browsing
                return ActivityType.Browsing;
            }

            return ActivityType.Unknown;
        }
        catch
        {
            return ActivityType.Unknown;
        }
    }


// -----------------------------
// SAVE TREND DATA
// -----------------------------

void SaveActivityData(ActivityType type, float duration)
    {
        // Make sure this activity exists
        if (Trends.ContainsKey(type))
        {
            // Add total time spent
            Trends[type].TotalTime += duration;

            // Increase session count
            Trends[type].Sessions++;

            // Record what hour this activity happened
            int currentHour = DateTime.Now.Hour;

            Trends[type].HourCounts[currentHour]++;

            UnityEngine.Debug.Log(
                type +
                " Total Time: " +
                Trends[type].TotalTime);

            UnityEngine.Debug.Log(
                type +
                " Sessions: " +
                Trends[type].Sessions);
        }
    }

    // -----------------------------
    // EXAMPLE TREND CHECKS
    // -----------------------------

    // Returns true if the player
    // commonly games at night
    /*public bool PlayerUsuallyGamesAtNight()
    {
        int nightGamingCount = 0;

        // Check hours from 8 PM -> 12 AM
        for (int i = 20; i < 24; i++)
        {
            nightGamingCount +=
                Trends[ActivityType.Gaming]
                .HourCounts[i];
        }

        // Arbitrary threshold
        return nightGamingCount > 5;
    }

    // Example:
    // Companion prepares snacks before gaming
    void ExampleCompanionReaction()
    {
        if (PlayerUsuallyGamesAtNight())
        {
            UnityEngine.Debug.Log(
                "Companion prepares gaming routine.");
        }
    }*/
}
