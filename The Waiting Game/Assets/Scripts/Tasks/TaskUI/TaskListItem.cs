using System;
using Tasks.ActiveTask;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Monads;
using Utilities.SaveLoad;

namespace Tasks.TaskUI
{
    /// <summary>
    /// A MonoBehaviour class that represents a UI element for displaying a task in a list.
    /// </summary>
    public class TaskListItem : MonoBehaviour
    {
        /// <summary>
        /// The button that, when clicked, will mark the task as completed.
        /// </summary>
        [SerializeField] private Button completionButton;

        /// <summary>
        /// The text element that displays the title of the task.
        /// </summary>
        [SerializeField] private TMP_Text titleText;

        /// <summary>
        /// A reference to the ActiveTaskButtonSwitcher component that manages the visibility of the active task buttons.
        /// </summary>
        [SerializeField] private ActiveTaskButtonSwitcher activeTaskButtonSwitcher;

        /// <summary>
        /// An optional field that holds a reference to the TaskDataManager that this UI element belongs to.
        /// </summary>
        private Optional<TaskDataManager> _taskDataManager;

        /// <summary>
        /// An optional field that holds a reference to the TaskDataStorage that this UI element belongs to.
        /// </summary>
        private Optional<IStorage<TaskData>> _taskDataStorage;

        /// <summary>
        /// An optional field that holds the TaskData associated with this UI element.
        /// </summary>
        private Optional<TaskData> _taskData;

        private void Awake()
        {
            // Initialize the optional TaskData to None when the component is first created.
            _taskData = Optional<TaskData>.None;

            completionButton.onClick.AddListener(OnCompletionButtonClicked);
        }

        private void OnDestroy()
        {
            // Unbind from the OnActiveTaskChanged event of the TaskDataManager's ActiveTaskData to prevent memory leaks.
            if (_taskDataManager.Unwrap(out var taskDataManager))
                taskDataManager.ActiveTaskData.OnActiveTaskChanged -= SetActiveButtonVisibility;
        }

        private void OnCompletionButtonClicked()
        {
            // TODO: Handle the completion button click event.
            if (!_taskDataStorage.Unwrap(out var taskDataStorage) || !_taskData.Unwrap(out var taskData))
            {
                Debug.LogError("Failed to unwrap task list or task data!");
                return;
            }

            // Remove the task from the list and storage when the completion button is clicked.
            taskDataStorage.RemoveItem(taskData.UniqueID);
            Destroy(gameObject);
        }

        public void SetTaskData(TaskDataManager taskDataManager, IStorage<TaskData> taskDataStorage, TaskData taskData)
        {
            // Set the optional fields to hold references to the provided TaskList and TaskData.
            _taskDataManager = Optional.Some(taskDataManager);
            _taskDataStorage = Optional.Some(taskDataStorage);
            _taskData = Optional.Some(taskData);

            // Store the provided TaskData in the optional field.
            titleText.text = taskData.title;

            // Bind to the OnActiveTaskChanged event of the TaskDataManager's ActiveTaskData to update the button visibility when the active task changes.
            taskDataManager.ActiveTaskData.OnActiveTaskChanged += SetActiveButtonVisibility;

            // Check the task data manager's active task data to determine which button to show.
            SetActiveButtonVisibility(taskDataManager.ActiveTaskData);
        }

        /// <summary>
        /// Sets the visibility of the active task buttons based on the currently active task in the TaskDataManager.
        /// Supposed to be called whenever the active task changes, to ensure the correct button is shown for this task.
        /// </summary>
        /// <param name="activeTaskData"></param>
        private void SetActiveButtonVisibility(ActiveTaskData activeTaskData)
        {
            if (!_taskData.Unwrap(out var taskData))
            {
                Debug.LogError("Failed to unwrap task data!");
                return;
            }

            // If there is no active task, show the "Set Active Task" button.
            if (!activeTaskData.TaskID.Unwrap(out var activeTaskID))
            {
                activeTaskButtonSwitcher.UpdateButtonVisibility(true);
                return;
            }

            // If the active task ID matches this task's unique ID, show the "Reset Active Task" button.
            // otherwise, show the "Set Active Task" button.
            activeTaskButtonSwitcher.UpdateButtonVisibility(activeTaskID != taskData.UniqueID);
        }

        #region Active Task Management

        /// <summary>
        /// Function to call when pressing the button to set this task as the active task.
        /// It will update the active task in the TaskDataManager.
        /// </summary>
        public void SetActiveTask()
        {
            if (!_taskDataManager.Unwrap(out var taskDataManager) || !_taskData.Unwrap(out var taskData))
            {
                Debug.LogError("Failed to unwrap task list or task data!");
                return;
            }

            // Set the active task in the TaskDataManager when this method is called.
            taskDataManager.ActiveTaskData.SetActiveTask(taskData.UniqueID, true);
        }

        /// <summary>
        /// Function to call when pressing the button to reset the active task.
        /// It will reset the active task in the TaskDataManager.
        /// This should only do anything if this task is currently the active task, otherwise it should do nothing.
        /// </summary>
        public void ResetActiveTask()
        {
            if (!_taskDataManager.Unwrap(out var taskDataManager))
            {
                Debug.LogError("Failed to unwrap task list!");
                return;
            }

            // If this is not the currently active task, do nothing.
            if (
                !taskDataManager.ActiveTaskData.TaskID.Unwrap(out var activeTaskID) ||
                !_taskData.Unwrap(out var taskData) || activeTaskID != taskData.UniqueID
            )
            {
                Debug.LogWarning("Attempted to reset active task, but this task is not the currently active task.");
                return;
            }

            // Reset the active task in the TaskDataManager when this method is called.
            taskDataManager.ActiveTaskData.SetActiveTask(new GUID(), true);
        }

        #endregion
    }
}