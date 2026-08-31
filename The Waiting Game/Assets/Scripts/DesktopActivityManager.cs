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

    float GetIdleTime()
    {
        LASTINPUTINFO info = new LASTINPUTINFO();
        info.cbSize = (uint)Marshal.SizeOf(info);

        if (!GetLastInputInfo(ref info))
            return 0f;

        uint idleTicks = (uint)Environment.TickCount - info.dwTime;
        return idleTicks / 1000f;
    }


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

    // Timer for how often detection happens
    float checkTimer;

    // Tracks how long the current activity
    // has been active
    float activityTimer;

    public MainMenu mainMenu;
    public UI_Shop shop;

    [HideInInspector] public float playTimer; // Checks playtime
    public TextMeshProUGUI playTimeText; // Displays playtime

    [HideInInspector] public float workTimer; // Checks amount of time spent working
    public TextMeshProUGUI workTimeText; // Displays work time


    // -----------------------------
    // START
    // -----------------------------

    void Awake()
    {
        gameProcessName = Application.productName.ToLowerInvariant();
    }

    void Start()
    {
        // Create trend data for every activity type
        foreach (ActivityType type in Enum.GetValues(typeof(ActivityType)))
        {
            Trends[type] = new ActivityTrend();
        }

        LoadPlayTime();

        // Start background activity loop instead of using checkTimer
        StartCoroutine(ActivityLoop());
    }

    IEnumerator ActivityLoop()
    {
        while (true)
        {
            ActivityType newActivity = DetectActivity();

            if (newActivity != CurrentActivity)
            {
                SaveActivityData(CurrentActivity, activityTimer);

                activityTimer = 0f;
                CurrentActivity = newActivity;

            #if UNITY_EDITOR
                UnityEngine.Debug.Log("New Activity: " + CurrentActivity);
            #endif

                if (activityDebugText != null)
                    activityDebugText.text = CurrentActivity.ToString();
            }

            yield return new WaitForSeconds(5f);
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
        if (mainMenu.gameObject.activeSelf && !shop.gameObject.activeSelf)
            mainMenu.CloseMenu();
        else if (!mainMenu.gameObject.activeSelf)
            mainMenu.OpenMenu();
        else if (mainMenu.gameObject.activeSelf && shop.gameObject.activeSelf)
            { 
                shop.gameObject.SetActive(false);
                mainMenu.ShopIsClosed();
            }
        }
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
        //Convert workTimer to an int but let it update as a float
        int totalSeconds = Mathf.FloorToInt(workTimer);

        int seconds = totalSeconds % 60;
        int minutes = (totalSeconds / 60) % 60;
        int hours = totalSeconds / 3600;

        workTimeText.text = "Work Time: " + hours + " hours " + minutes + " minutes " + seconds + " seconds ";
    }

    public void SavePlayTime()
    {
        PlayerPrefs.SetInt("PlayTime", Mathf.FloorToInt(playTimer));
        PlayerPrefs.SetInt("WorkTime", Mathf.FloorToInt(workTimer));
        PlayerPrefs.Save();
    }

    public void LoadPlayTime()
    {
        playTimer = PlayerPrefs.GetInt("PlayTime", 0);
        workTimer = PlayerPrefs.GetInt("WorkTime", 0);
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

        #if UNITY_EDITOR
            UnityEngine.Debug.Log(windowTitle);
        #endif

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

        // ===== REVISION 8 =====
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
