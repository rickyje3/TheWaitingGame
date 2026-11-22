
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActivityTrackingUI : MonoBehaviour
{
    [Header("References")]
    public DesktopActivityManager activityManager;

    [Header("Add Activity")]
    public TMP_InputField activityNameInput;
    public TMP_Dropdown detectionTargetDropdown;
    public TMP_Dropdown activityTypeDropdown;
    public TMP_Text statusText;

    [Header("Existing Activities")]
    public Transform entriesContainer;
    public ActivityEntryUI entryPrefab;

    private DesktopActivityManager.DetectionTarget selectedTarget;

    private DesktopActivityManager.ActivityType selectedActivity;

    private readonly DesktopActivityManager.ActivityType[]
        availableActivityTypes =
    {
        DesktopActivityManager.ActivityType.Working,
        DesktopActivityManager.ActivityType.Gaming,
        DesktopActivityManager.ActivityType.Watching,
        DesktopActivityManager.ActivityType.Browsing,
        DesktopActivityManager.ActivityType.Jamming,
    };

    private void Start()
    {
        SetupDropdowns();
        RefreshActivityList();
    }

    private void SetupDropdowns()
    {
        // Detection target dropdown.
        detectionTargetDropdown.ClearOptions();

        detectionTargetDropdown.AddOptions(
            new List<string>
            {
                "Application",
                "Website"
            });

        // Activity type dropdown.
        activityTypeDropdown.ClearOptions();

        List<string> activityNames = new List<string>();

        foreach (var type in availableActivityTypes)
        {
            activityNames.Add(type.ToString());
        }

        activityTypeDropdown.AddOptions(activityNames);

        // Register dropdown callbacks.
        detectionTargetDropdown.onValueChanged.AddListener(
            OnDetectionTargetChanged);

        activityTypeDropdown.onValueChanged.AddListener(
            OnActivityTypeChanged);

        // Set initial selections.
        detectionTargetDropdown.value = 0;
        activityTypeDropdown.value = 0;

        OnDetectionTargetChanged(0);
        OnActivityTypeChanged(0);
    }

    private void OnDetectionTargetChanged(int index)
    {
        selectedTarget =
            (DesktopActivityManager.DetectionTarget)index;
    }

    private void OnActivityTypeChanged(int index)
    {
        if (index < 0 || index >= availableActivityTypes.Length)
            return;

        selectedActivity = availableActivityTypes[index];
    }

    public void AddActivity()
    {
        string keyword = activityNameInput.text.Trim();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            SetStatus("Please enter an application or website name.");
            return;
        }

        bool added = activityManager.AddCustomActivityItem(
            keyword,
            selectedActivity,
            selectedTarget
        );

        if (!added)
        {
            SetStatus("This entry already exists in that category.");
            return;
        }

        activityNameInput.text = "";

        SetStatus("Activity successfully added!");

        RefreshActivityList();
    }

    public void RefreshActivityList()
    {
        // Clear existing UI entries.
        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }

        // Display every custom category.
        foreach (DesktopActivityManager.ActivityRule rule
                 in activityManager.customActivityRules)
        {
            CreateEntries(
                rule,
                rule.applications,
                DesktopActivityManager.DetectionTarget.Application
            );

            CreateEntries(
                rule,
                rule.websites,
                DesktopActivityManager.DetectionTarget.Website
            );
        }
    }

    private void CreateEntries(
        DesktopActivityManager.ActivityRule rule,
        List<DesktopActivityManager.TrackedActivityItem> items,
        DesktopActivityManager.DetectionTarget target)
    {
        for (int i = 0; i < items.Count; i++)
        {
            int index = i;

            ActivityEntryUI entry = Instantiate(
                entryPrefab,
                entriesContainer
            );

            entry.Setup(
                items[index].keyword,
                rule.activityType.ToString(),
                target.ToString(),
                items[index].enabled,

                // Enable / disable callback.
                enabled =>
                {
                    activityManager.SetCustomActivityItemEnabled(
                        rule.activityType,
                        target,
                        index,
                        enabled
                    );
                },

                // Delete callback.
                () =>
                {
                    activityManager.RemoveCustomActivityItem(
                        rule.activityType,
                        target,
                        index
                    );

                    RefreshActivityList();
                }
            );
        }
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    private void OnDestroy()
    {
        if (detectionTargetDropdown != null)
        {
            detectionTargetDropdown.onValueChanged.RemoveListener(
                OnDetectionTargetChanged
            );
        }

        if (activityTypeDropdown != null)
        {
            activityTypeDropdown.onValueChanged.RemoveListener(
                OnActivityTypeChanged
            );
        }
    }
}