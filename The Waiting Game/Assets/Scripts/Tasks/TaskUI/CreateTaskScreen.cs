using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tasks.TaskUI
{
    public class CreateTaskScreen : MonoBehaviour, ITaskDataFactory
    {
        [SerializeField] private GameObject owningGameObject;
        [SerializeField] private TMP_InputField titleText;
        [SerializeField] private TMP_InputField descriptionText;
        [SerializeField] private Button backButton;
        [SerializeField] private Button confirmButton;

        private Optional<GUID> _uniqueID;
        private Optional<ITaskDataStorage> _taskDataStorage;
        private Optional<GameObject> _previousScreen;

        private void Awake()
        {
            // Initialize the optional unique ID to None when the component is first created.
            _uniqueID = Optional<GUID>.None;

            // Initialize the optional TaskDataStorage to None when the component is first created.
            _taskDataStorage = Optional<ITaskDataStorage>.None;

            // Initialize the callbacks
            backButton.onClick.AddListener(OnBackButtonClicked);
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        private void Start()
        {
            // Initialize the unique ID to a new GUID when the component is first created.
            SetGuid(Optional<GUID>.None);
        }

        /// <summary>
        /// Creates a new TaskData instance with the provided title and description.
        /// </summary>
        /// <returns>The created TaskData wrapped in an Optional.</returns>
        public Optional<TaskData> CreateTaskData()
        {
            // Ensure that the unique ID is not empty before creating the TaskData.
            if (!_uniqueID.Unwrap(out var guid))
            {
                Debug.LogWarning("Unique ID is empty. Generating a new one.");
                SetGuid(Optional<GUID>.None);

                // Unwrap the unique ID again after generating a new one.
                guid = _uniqueID.Value;
            }

            // Create a new TaskData instance with the provided title and description.
            var taskData = new TaskData(guid)
            {
                title = titleText.text,
                description = descriptionText.text
            };

            // Return the created TaskData wrapped in an Optional.
            return taskData;
        }

        /// <summary>
        /// Sets the unique ID for the task.
        /// </summary>
        /// <param name="guid">The GUID to use, or an empty Optional to generate a new one.</param>
        public void SetGuid(Optional<GUID> guid)
        {
            // If we pass in a valid GUID, use it; otherwise, generate a new one.
            _uniqueID = guid.MatchValueOrNone(GUID.Generate);
        }


        private void OnConfirmButtonClicked()
        {
            // If the task storage service is none, we cannot save the task data, so we log an error and return early.
            if (!_taskDataStorage.Unwrap(out var taskDataStorage))
            {
                Debug.LogError("TaskDataStorage is not set. Cannot save task data.");
                return;
            }

            // Create the task data using the factory method.
            var taskDataOptional = CreateTaskData();
            if (!taskDataOptional.Unwrap(out var taskData))
            {
                Debug.LogError("Failed to create task data. Cannot save.");
                return;
            }

            // Save the created TaskData to the storage.
            var bSaved = taskDataStorage.SaveItem(taskData);

            // Log the result of the save operation.
            if (!bSaved)
            {
                Debug.LogError("Failed to save task data to storage.");
                return;
            }

            //
            Debug.Log($"Task data saved successfully: {taskData.title}");

            // Close the screen after saving the task data.
            CloseScreen();
        }

        private void OnBackButtonClicked()
        {
            // Close the screen when the back button is clicked.
            CloseScreen();
        }

        private void CloseScreen()
        {
            // TODO: Implement the logic to close the create task screen,
            // such as disabling the GameObject or triggering an animation.

            // Disable the owning GameObject to hide the create task screen.
            owningGameObject.SetActive(false);

            // If a previous screen is set, enable it to return to the previous UI state.
            if (_previousScreen.Unwrap(out var previousScreen))
                previousScreen.SetActive(true);
        }

        public void OpenCreateTaskScreen(GameObject previousScreen, ITaskDataStorage taskDataStorage)
        {
            // Set the task data storage for this screen.
            _taskDataStorage = Optional.Some(taskDataStorage);

            // Set the previous screen.
            _previousScreen = Optional.Some(previousScreen);

            // Reset the input fields and unique ID when opening the screen.
            titleText.text = string.Empty;
            descriptionText.text = string.Empty;
            SetGuid(Optional<GUID>.None);
        }
    }
}