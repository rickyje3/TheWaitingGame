using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class DesktopActivityManager : MonoBehaviour
{
    public TextMeshProUGUI activityDebugText;

    // Reuse the same StringBuilder instead of allocating one every check
    private readonly StringBuilder windowTitleBuffer = new StringBuilder(256);

    // Cache our own process name
    private string gameProcessName;

    // Cache the last foreground process
    private uint lastPID;
    private string lastProcessName = "";

    // Timer for updating UI once per second
    private float uiTimer;

    public MoneyManager moneyManager;

    private int totalIntervalsPassed; // Tracks the total number of working intervals passed
    public int intervalMinutes = 10; // The interval duration in minutes
    private int nextIntervalTime;

    [Header("Input Reward Tracking")]
    public float inputRewardCooldown = 0.1f;

    private float nextInputRewardTime;


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

    [StructLayout(LayoutKind.Sequential)]
    struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    [DllImport("user32.dll")]
    static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);


    // Checking input offscreen

    private uint lastProcessedInputTick;
    private bool hasProcessedInitialInput;
    private bool pendingSystemInput;
    public bool HasNewSystemInput { get; private set; }


    // ACTIVITY TYPES
    public enum ActivityType
    {
        Working,
        Gaming,
        Watching,
        Browsing,
        Jamming,
        Idle,
        Gooning,
        Unknown
    }


    public enum DetectionTarget
    {
        Application,
        Website
    }

    [System.Serializable]
    public class TrackedActivityItem
    {
        public string keyword;
        public bool enabled = true;
    }

    [System.Serializable]
    public class ActivityRule
    {
        public ActivityType activityType;

        public List<TrackedActivityItem> applications =
            new List<TrackedActivityItem>();

        public List<TrackedActivityItem> websites =
            new List<TrackedActivityItem>();
    }

    [System.Serializable]
    private class ActivityRuleSaveData
    {
        public List<ActivityRule> rules =
            new List<ActivityRule>();
    }

    [Header("Activity Detection Rules")]

    [Tooltip("Default activity categories and their built-in entries.")]
    public List<ActivityRule> defaultActivityRules =
    new List<ActivityRule>();

    [Tooltip("Additional entries configured by the player.")]
    public List<ActivityRule> customActivityRules =
        new List<ActivityRule>();

    private const string ActivityRulesKey = "CustomActivityRules";

    // Browsers whose window titles can be checked for websites.
    private readonly string[] browserProcesses =
    {
    "chrome",
    "firefox",
    "msedge",
    "brave",
    "opera"
    };

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

    // Tracks how long the current activity
    // has been active
    float activityTimer;

    public MainMenu mainMenu;
    public UI_Shop shop;

    [HideInInspector] public float playTimer; // Checks playtime
    public TextMeshProUGUI playTimeText; // Displays playtime

    [HideInInspector] public float workTimer; // Checks amount of time spent working
    public TextMeshProUGUI workTimeText; // Displays work time

    public float timeToIdle = 10.0f;  // 10 seconds




    // -----------------------------
    // START
    // -----------------------------

    void Awake()
    {
        gameProcessName = Application.productName.ToLowerInvariant();
    }

    void Start()
    {
        Application.runInBackground = true;

        /*// Create trend data for every activity type
        foreach (ActivityType type in Enum.GetValues(typeof(ActivityType)))
        {
            Trends[type] = new ActivityTrend();
        }*/

        CurrentActivity = ActivityType.Unknown;

        if (activityDebugText != null)
            activityDebugText.text = CurrentActivity.ToString();

        LoadPlayTime();

        LoadCustomActivityRules();

        // Start background activity loop instead of using checkTimer
        StartCoroutine(ActivityLoop());
    }



    IEnumerator ActivityLoop()
    {
        while (true)
        {
            ActivityType newActivity;

            // Windows detects input from anywhere on the computer.
            if (GetSystemIdleSeconds() >= timeToIdle)
            {
                newActivity = ActivityType.Idle;
            }
            else
            {
                // Classify the currently focused application.
                newActivity = DetectActivity();
            }

            if (newActivity != CurrentActivity)
            {
                SaveActivityData(CurrentActivity, activityTimer);

                activityTimer = 0f;
                CurrentActivity = newActivity;

                if (activityDebugText != null)
                {
                    activityDebugText.text = CurrentActivity.ToString();
                }

                UnityEngine.Debug.Log("New Activity: " + CurrentActivity);
            }

            // Wait another 2 seconds before checking again
            yield return new WaitForSecondsRealtime(2f);
        }
    }






    // -----------------------------
    // UPDATE LOOP
    // -----------------------------

    void Update()
    {
        // Increase timers every frame
        // Only keep timers that truly need frame updates
        activityTimer += Time.deltaTime;
        playTimer += Time.deltaTime;

        // ===== REVISION 5 =====
        // Update UI only once per second
        uiTimer += Time.deltaTime;

        if (uiTimer >= 1f)
        {
          uiTimer = 0f;

           checkPlayTime();
           checkWorkTime();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (mainMenu.isActivityOpen)
                {
                    mainMenu.ActivityIsClosed();
                }
                else if (mainMenu.isShopOpen)
                {
                    mainMenu.ShopIsClosed();
                }
                else if (mainMenu.isUpgradesOpen)
                {
                    mainMenu.UpgradesIsClosed();
                }
                else if (mainMenu.isDailiesOpen)
                {
                    mainMenu.DailiesIsClosed();
                }
                else if(mainMenu.isMenuOpen)
                {
                    mainMenu.CloseMenu();
                }
                else if (!mainMenu.isMenuOpen)
                {
                    mainMenu.OpenMenu();
                }
            }
        }

        CheckForSystemInput();
    }

    public void checkPlayTime()
    {
        //Convert playTimer to an int but let it update as a float
        int totalSeconds = Mathf.FloorToInt(playTimer);

        int seconds = totalSeconds % 60;
        int minutes = (totalSeconds / 60) % 60;
        int hours = totalSeconds / 3600;

        playTimeText.text = "Playtime: " + hours + " hours " + minutes + " minutes " + seconds + " seconds ";

        //UnityEngine.Debug.Log("Playtime: " + hours + " hours " + minutes + " minutes " + seconds + " seconds ");
    }

    public void checkWorkTime()
    {
        int totalSeconds = Mathf.FloorToInt(workTimer);

        int seconds = totalSeconds % 60;
        int minutes = (totalSeconds / 60) % 60;
        int hours = totalSeconds / 3600;

        workTimeText.text =
            "Work Time: " +
            hours + " hours " +
            minutes + " minutes " +
            seconds + " seconds ";

        // Check if we've reached the next interval
        if (workTimer >= nextIntervalTime)
        {
            IncrementAndSave();

            // Set the next interval
            nextIntervalTime += intervalMinutes * 60;
        }
    }

    public void SavePlayTime()
    {
        PlayerPrefs.SetInt("PlayTime", Mathf.FloorToInt(playTimer));
        PlayerPrefs.SetInt("WorkTime", Mathf.FloorToInt(workTimer));
        PlayerPrefs.SetInt("IntervalCount", totalIntervalsPassed);

        PlayerPrefs.Save();
    }

    public void LoadPlayTime()
    {
        playTimer = PlayerPrefs.GetInt("PlayTime", 0);
        workTimer = PlayerPrefs.GetInt("WorkTime", 0);
        totalIntervalsPassed = PlayerPrefs.GetInt("IntervalCount", 0);

        // Find the next interval AFTER the saved work time.
        int intervalLength = intervalMinutes * 60;

        nextIntervalTime =
            ((Mathf.FloorToInt(workTimer) / intervalLength) + 1)
            * intervalLength;

        UnityEngine.Debug.Log(
            $"Loaded WorkTime: {workTimer}s | " +
            $"Intervals Passed: {totalIntervalsPassed} | " +
            $"Next Interval: {nextIntervalTime}s"
        );
    }



    private float GetSystemIdleSeconds()
    {
        LASTINPUTINFO inputInfo = new LASTINPUTINFO();

        inputInfo.cbSize = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO));

        if (!GetLastInputInfo(ref inputInfo))
            return 0f;

        // Both values represent milliseconds since Windows started.
        // Unsigned subtraction also handles the tick counter wrapping.
        uint elapsedMilliseconds =
            unchecked((uint)Environment.TickCount - inputInfo.dwTime);

        return elapsedMilliseconds / 1000f;
    }



    private void CheckForSystemInput()
    {
        // Check keyboard presses, excluding mouse-button virtual keys.
        // The low bit detects a new press rather than a held key.
        for (int key = 0x08; key <= 0xFE; key++)
        {
            // Exclude mouse buttons from the keyboard loop.
            if (key == 0x01 || key == 0x02 ||
                key == 0x04 || key == 0x05 ||
                key == 0x06)
            {
                continue;
            }

            // Exclude Escape from earning money.
            if (key == 0x1B)
                continue;

            if ((GetAsyncKeyState(key) & 1) != 0)
            {
                pendingSystemInput = true;
                break;
            }
        }

        // Check mouse buttons separately.
        int[] mouseButtons = { 0x01, 0x02, 0x04, 0x05, 0x06 };

        foreach (int button in mouseButtons)
        {
            if ((GetAsyncKeyState(button) & 1) != 0)
            {
                pendingSystemInput = true;
                break;
            }
        }

        HasNewSystemInput = pendingSystemInput;
    }


    public bool TryConsumeSystemInput()
    {
        if (!pendingSystemInput)
            return false;

        // Consume the detected input so it cannot be reused.
        pendingSystemInput = false;

        // Prevent rapid repeated rewards from continuous input.
        if (Time.unscaledTime < nextInputRewardTime)
            return false;

        nextInputRewardTime = Time.unscaledTime + inputRewardCooldown;

        return true;
    }


    private void IncrementAndSave()
    {
        totalIntervalsPassed++;

        PlayerPrefs.SetInt(
            "IntervalCount",
            totalIntervalsPassed);

        PlayerPrefs.SetInt(
            "PlayTime",
            Mathf.FloorToInt(playTimer));

        PlayerPrefs.SetInt(
            "WorkTime",
            Mathf.FloorToInt(workTimer));

        PlayerPrefs.Save();

        UnityEngine.Debug.Log(
            $"Interval triggered! " +
            $"Total count: {totalIntervalsPassed}");

        moneyManager.UpgradeInputMult();
    }


    // DETECT CURRENT PLAYER ACTIVITY

    ActivityType DetectActivity()
    {
        IntPtr hwnd = GetForegroundWindow();

        if (hwnd == IntPtr.Zero)
            return ActivityType.Unknown;

        GetWindowThreadProcessId(hwnd, out uint pid);

        if (pid == 0)
            return ActivityType.Unknown;

        // Get window title.
        windowTitleBuffer.Clear();

        GetWindowText(
            hwnd,
            windowTitleBuffer,
            windowTitleBuffer.Capacity
        );

        string windowTitle =
            windowTitleBuffer.ToString().ToLowerInvariant();

        string processName;

        // Reuse cached process name.
        if (pid == lastPID)
        {
            processName = lastProcessName;
        }
        else
        {
            try
            {
                using (Process process =
                    Process.GetProcessById((int)pid))
                {
                    processName =
                        process.ProcessName.ToLowerInvariant();
                }

                lastPID = pid;
                lastProcessName = processName;
            }
            catch
            {
                return ActivityType.Unknown;
            }
        }

        // Ignore our own game.
        if (processName.Contains(gameProcessName))
            return ActivityType.Gaming;

        // Determine whether the foreground application is a browser.
        bool isBrowser = ContainsAny(
            processName,
            browserProcesses
        );

        // Check custom rules first.
        ActivityType? matchedActivity =
            CheckActivityRules(
                customActivityRules,
                processName,
                windowTitle,
                isBrowser
            );

        if (matchedActivity.HasValue)
            return matchedActivity.Value;

        // Then check built-in rules.
        matchedActivity =
            CheckActivityRules(
                defaultActivityRules,
                processName,
                windowTitle,
                isBrowser
            );

        if (matchedActivity.HasValue)
            return matchedActivity.Value;

        // Unrecognized browser websites default to Browsing.
        if (isBrowser)
            return ActivityType.Browsing;

        return ActivityType.Unknown;
    }

    private ActivityType? CheckActivityRules(
    List<ActivityRule> rules,
    string processName,
    string windowTitle,
    bool isBrowser)
    {
        foreach (ActivityRule rule in rules)
        {
            if (rule == null)
                continue;

            // Check application entries.
            foreach (TrackedActivityItem item in rule.applications)
            {
                if (item == null || !item.enabled)
                    continue;

                if (string.IsNullOrWhiteSpace(item.keyword))
                    continue;

                if (processName.Contains(
                    item.keyword.Trim().ToLowerInvariant()))
                {
                    return rule.activityType;
                }
            }

            // Check website entries.
            if (isBrowser)
            {
                foreach (TrackedActivityItem item in rule.websites)
                {
                    if (item == null || !item.enabled)
                        continue;

                    if (string.IsNullOrWhiteSpace(item.keyword))
                        continue;

                    if (windowTitle.Contains(
                        item.keyword.Trim().ToLowerInvariant()))
                    {
                        return rule.activityType;
                    }
                }
            }
        }

        return null;
    }


    public bool AddCustomActivityItem(
        string keyword,
        ActivityType activityType,
        DetectionTarget target)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return false;

        keyword = keyword.Trim();

        ActivityRule sourceRule = null;
        TrackedActivityItem existingItem = null;

        // Search for an existing custom entry.
        foreach (ActivityRule rule in customActivityRules)
        {
            List<TrackedActivityItem> items =
                target == DetectionTarget.Application
                ? rule.applications
                : rule.websites;

            existingItem = items.Find(item =>
                item != null &&
                string.Equals(
                    item.keyword,
                    keyword,
                    StringComparison.OrdinalIgnoreCase));

            if (existingItem != null)
            {
                sourceRule = rule;
                break;
            }
        }

        // Don't add an identical entry twice.
        if (sourceRule != null &&
            sourceRule.activityType == activityType)
        {
            return false;
        }

        // Remove an existing entry if its category is changing.
        if (sourceRule != null)
        {
            if (target == DetectionTarget.Application)
                sourceRule.applications.Remove(existingItem);
            else
                sourceRule.websites.Remove(existingItem);
        }

        // Find or create the destination category.
        ActivityRule destination = customActivityRules.Find(
            rule => rule.activityType == activityType);

        if (destination == null)
        {
            destination = new ActivityRule
            {
                activityType = activityType
            };

            customActivityRules.Add(destination);
        }

        // Create the new entry.
        TrackedActivityItem newItem = new TrackedActivityItem
        {
            keyword = keyword,
            enabled = true
        };

        if (target == DetectionTarget.Application)
            destination.applications.Add(newItem);
        else
            destination.websites.Add(newItem);

        SaveCustomActivityRules();

        return true;
    }

    public void RemoveCustomActivityItem(
        ActivityType activityType,
        DetectionTarget target,
        int index)
    {
        ActivityRule rule = customActivityRules.Find(
            r => r.activityType == activityType);

        if (rule == null)
            return;

        List<TrackedActivityItem> items =
            target == DetectionTarget.Application
            ? rule.applications
            : rule.websites;

        if (index < 0 || index >= items.Count)
            return;

        items.RemoveAt(index);

        // Remove empty custom categories.
        if (rule.applications.Count == 0 &&
            rule.websites.Count == 0)
        {
            customActivityRules.Remove(rule);
        }

        SaveCustomActivityRules();
    }

    public void SetCustomActivityItemEnabled(
        ActivityType activityType,
        DetectionTarget target,
        int index,
        bool enabled)
    {
        ActivityRule rule = customActivityRules.Find(
            r => r.activityType == activityType);

        if (rule == null)
            return;

        List<TrackedActivityItem> items =
            target == DetectionTarget.Application
            ? rule.applications
            : rule.websites;

        if (index < 0 || index >= items.Count)
            return;

        items[index].enabled = enabled;

        SaveCustomActivityRules();
    }


    private bool ContainsKeyword(
        ActivityRule rule,
        string keyword,
        DetectionTarget target)
    {
        List<TrackedActivityItem> items =
            target == DetectionTarget.Application
            ? rule.applications
            : rule.websites;

        return items.Exists(item =>
            item != null &&
            string.Equals(
                item.keyword,
                keyword,
                StringComparison.OrdinalIgnoreCase));
    }

    private void AddItemToRule(
    ActivityRule rule,
    string keyword,
    DetectionTarget target)
    {
        TrackedActivityItem item = new TrackedActivityItem
        {
            keyword = keyword,
            enabled = true
        };

        if (target == DetectionTarget.Application)
            rule.applications.Add(item);
        else
            rule.websites.Add(item);
    }


    public void SaveCustomActivityRules()
    {
        ActivityRuleSaveData data = new ActivityRuleSaveData
        {
            rules = customActivityRules
        };

        PlayerPrefs.SetString(
            ActivityRulesKey,
            JsonUtility.ToJson(data));

        PlayerPrefs.Save();
    }


    public void LoadCustomActivityRules()
    {
        if (!PlayerPrefs.HasKey(ActivityRulesKey))
            return;

        ActivityRuleSaveData data =
            JsonUtility.FromJson<ActivityRuleSaveData>(
                PlayerPrefs.GetString(ActivityRulesKey));

        if (data != null && data.rules != null)
            customActivityRules = data.rules;
    }


    public void RemoveCustomActivityRule(int index)
    {
        if (index < 0 || index >= customActivityRules.Count)
            return;

        customActivityRules.RemoveAt(index);

        SaveCustomActivityRules();
    }



    // ===== REVISION 9 =====
    bool ContainsAny(string value, params string[] keywords)
    {
        foreach (string keyword in keywords)
        {
            if (value.Contains(keyword))
                return true;
        }

        return false;
    }

    // ===== REVISION 10 =====
    ActivityType DetectBrowserActivity(string windowTitle)
    {
        if (ContainsAny(windowTitle,
            "youtube",
            "netflix",
            "twitch",
            "hulu",
            "disney+"))
        {
            return ActivityType.Watching;
        }

        if (windowTitle.Contains("roblox"))
            return ActivityType.Gaming;

        if (ContainsAny(windowTitle,
            "docs",
            "sheets",
            "trello",
            "clickup",
            "microsoft",
            "github",
            "notion"))
        {
            return ActivityType.Working;
        }

        if (ContainsAny(windowTitle,
            "xvideos",
            "pornhub",
            "redtube",
            "youporn",
            "rule34"))
        {
            return ActivityType.Gooning;
        }

        return ActivityType.Browsing;
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
