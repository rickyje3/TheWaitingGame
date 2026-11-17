using System;
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
        /// An optional field that holds the TaskData associated with this UI element.
        /// </summary>
        private Optional<TaskData> _taskData;

        private void Awake()
        {
            // Initialize the optional TaskData to None when the component is first created.
            _taskData = Optional<TaskData>.None;

            completionButton.onClick.AddListener(OnCompletionButtonClicked);
        }

        private void OnCompletionButtonClicked()
        {
            // TODO: Handle the completion button click event.
        }

        public void SetTaskData(TaskData taskData)
        {
            _taskData = Optional.Some(taskData);

            // Store the provided TaskData in the optional field.
            titleText.text = taskData.title;
        }

    }
}