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
            if (mainMenu.gameObject.activeSelf)
                mainMenu.CloseMenu();
            else if (!mainMenu.isShopOpen && !mainMenu.isUpgradesOpen && !mainMenu.isDailiesOpen)
                mainMenu.OpenMenu();
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

        // Reuse StringBuilder
        windowTitleBuffer.Clear();
        GetWindowText(hwnd, windowTitleBuffer, windowTitleBuffer.Capacity);

        string windowTitle = windowTitleBuffer.ToString().ToLowerInvariant();

       /* #if UNITY_EDITOR
            UnityEngine.Debug.Log(windowTitle);
        #endif*/

        if (pid == 0)
            return ActivityType.Unknown;

        string processName;

        // ===== REVISION 7 =====
        // Cache process lookup if foreground app didn't change
        if (pid == lastPID)
        {
            processName = lastProcessName;
        }
        else
        {
            try
            {
                Process process = Process.GetProcessById((int)pid);

                processName = process.ProcessName.ToLowerInvariant();

                lastPID = pid;
                lastProcessName = processName;
            }
            catch
            {
                return ActivityType.Unknown;
            }
        }

        // Ignore our own game
        if (processName.Contains(gameProcessName))
            return CurrentActivity;

        #if UNITY_EDITOR
            UnityEngine.Debug.Log("Detected Process: " + processName);
        #endif


        // Use helper instead of giant OR chains
        if (ContainsAny(processName,
            "unity",
            "code",
            "visual studio",
            "github",
            "photoshop",
            "unreal",
            "maya",
            "adobe",
            "word",
            "excel",
            "powerpoint",
            "zoom",
            "teams",
            "blender"))
        {
            return ActivityType.Working;
        }

        if (ContainsAny(processName,
            "steam",
            "roblox",
            "minecraft"))
        {
            return ActivityType.Gaming;
        }

        if (ContainsAny(processName,
            "spotify",
            "applemusic",
            "pandora"))
        {
            return ActivityType.Jamming;
        }

        if (ContainsAny(processName,
            "chrome",
            "firefox",
            "edge",
            "brave",
            "opera"))
        {
            return DetectBrowserActivity(windowTitle);
        }

        return ActivityType.Unknown;
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
