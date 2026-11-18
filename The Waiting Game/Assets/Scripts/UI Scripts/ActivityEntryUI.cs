
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivityEntryUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text activityNameText;
    public TMP_Text activityTypeText;
    public TMP_Text detectionTargetText;

    public Toggle enabledToggle;
    public Button deleteButton;

    private Action<bool> onToggleChanged;
    private Action onDelete;

    public void Setup(
        string activityName,
        string activityType,
        string detectionTarget,
        bool isEnabled,
        Action<bool> toggleCallback,
        Action deleteCallback)
    {
        activityNameText.text = activityName;
        activityTypeText.text = activityType;
        detectionTargetText.text = detectionTarget;

        onToggleChanged = toggleCallback;
        onDelete = deleteCallback;

        enabledToggle.SetIsOnWithoutNotify(isEnabled);

        enabledToggle.onValueChanged.RemoveAllListeners();

        enabledToggle.onValueChanged.AddListener(
            value => onToggleChanged?.Invoke(value)
        );

        deleteButton.onClick.RemoveAllListeners();

        deleteButton.onClick.AddListener(
            () => onDelete?.Invoke()
        );
    }
}