using UnityEngine;
using Kirurobo;

public class DesktopWindowManager : MonoBehaviour
{
    [Header("UniWindowController")]
    [SerializeField] private UniWindowController windowController;


    // The monitor the window is currently locked to.
    public int currentMonitor = 0;

    //private bool initialized = false;


    // =========================================================
    // FIND CURRENT MONITOR
    // =========================================================

    private int FindCurrentMonitor()
    {
        Vector2 windowPosition =
            windowController.windowPosition;

        Vector2 windowSize =
            windowController.windowSize;


        // Get the center of the window.
        Vector2 windowCenter =
            windowPosition +
            windowSize / 2f;


        int monitorCount =
            UniWindowController.GetMonitorCount();


        for (int i = 0; i < monitorCount; i++)
        {
            Rect monitor =
                UniWindowController.GetMonitorRect(i);


            if (monitor.Contains(windowCenter))
            {
                return i;
            }
        }


        return 0;
    }


    public void MoveWindowToMonitor(int monitorIndex)
    {
        int monitorCount =
            UniWindowController.GetMonitorCount();


        if (monitorCount <= 0)
            return;


        monitorIndex =
            Mathf.Clamp(
                monitorIndex,
                0,
                monitorCount - 1
            );


        Rect targetMonitor =
            UniWindowController.GetMonitorRect(
                monitorIndex
            );


        if (targetMonitor == Rect.zero)
        {
            Debug.LogWarning(
                "Could not get monitor rectangle."
            );

            return;
        }


        Vector2 windowSize =
            windowController.windowSize;


        // -----------------------------------------------------
        // Center the window on the target monitor.
        // -----------------------------------------------------

        float x =
            targetMonitor.x +
            (targetMonitor.width -
             windowSize.x) / 2f;


        float y =
            targetMonitor.y +
            (targetMonitor.height -
             windowSize.y) / 2f;


        Vector2 newPosition =
            new Vector2(x, y);


        // -----------------------------------------------------
        // Change the locked monitor.
        // -----------------------------------------------------

        currentMonitor =
            monitorIndex;


        // -----------------------------------------------------
        // Move the window.
        // -----------------------------------------------------

        windowController.windowPosition =
            newPosition;


        Debug.Log(
            "Moved window to monitor " +
            monitorIndex
        );
    }


    // =========================================================
    // CENTER CURRENT MONITOR
    // =========================================================

    public void CenterWindowOnCurrentMonitor()
    {
        MoveWindowToMonitor(currentMonitor);
    }


    // =========================================================
    // SWITCH MONITOR
    // =========================================================

    public void SwitchMonitor()
    {
        int monitorCount =
            UniWindowController.GetMonitorCount();


        if (monitorCount <= 1)
        {
            Debug.Log(
                "Only one monitor detected."
            );

            return;
        }


        int nextMonitor =
            currentMonitor + 1;


        if (nextMonitor >= monitorCount)
        {
            nextMonitor = 0;
        }


        MoveWindowToMonitor(nextMonitor);

        currentMonitor = nextMonitor;
    }


    // =========================================================
    // PUBLIC ACCESSORS
    // =========================================================

    public int GetCurrentMonitor()
    {
        return currentMonitor;
    }


    public int GetMonitorCount()
    {
        return UniWindowController.GetMonitorCount();
    }
}