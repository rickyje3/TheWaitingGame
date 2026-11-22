using Tasks.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        /// An optional field that holds a reference to the TaskDataStorage that this UI element belongs to.
        /// </summary>
        private Optional<ITaskDataStorage> _taskDataStorage;

        /// <summary>
        /// An optional field that holds the TaskData associated with this UI element.
        /// </summary>
        private Optional<TaskData.TaskData> _taskData;

        private void Awake()
        {
            // Initialize the optional TaskData to None when the component is first created.
            _taskData = Optional<TaskData.TaskData>.None;

            completionButton.onClick.AddListener(OnCompletionButtonClicked);
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

        public void SetTaskData(ITaskDataStorage taskDataStorage, TaskData.TaskData taskData)
        {
            // Set the optional fields to hold references to the provided TaskList and TaskData.
            _taskDataStorage = Optional.Some(taskDataStorage);
            _taskData = Optional.Some(taskData);

            // Store the provided TaskData in the optional field.
            titleText.text = taskData.title;
        }
    }
}