using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tasks.TaskUI
{
    public class ActiveTaskButtonSwitcher : MonoBehaviour
    {
        [SerializeField] private Button setActiveTaskButton;
        [SerializeField] private Button resetActiveTaskButton;

        public void UpdateButtonVisibility(bool bShowActivateButton)
        {
            // Update the button visibility based on the boolean flag.
            if (bShowActivateButton)
                SwitchToActiveTaskButton();
            else
                SwitchToResetActiveTaskButton();
        }

        private void SwitchToActiveTaskButton()
        {
            SetButtonEnabled(setActiveTaskButton, true);
            SetButtonEnabled(resetActiveTaskButton, false);
        }

        private void SwitchToResetActiveTaskButton()
        {
            SetButtonEnabled(setActiveTaskButton, false);
            SetButtonEnabled(resetActiveTaskButton, true);
        }

        private void SetButtonEnabled(Button button, bool isEnabled)
        {
            button.gameObject.SetActive(isEnabled);
            button.interactable = isEnabled;
        }
    }
}