using System;
using Alchemy.Inspector;
using Tasks.ActiveTask;
using UnityEngine;
using Utilities.Monads;
using Utilities.SaveLoad;

namespace Tasks
{
    public class TaskDataManager : MonoBehaviour
    {
        /// <summary>
        /// The data storage that holds the task data to be displayed in the list.
        /// This is NOT an optional field, as the TaskList requires a valid data storage to function correctly.
        /// </summary>
        [SerializeReference] private IStorage<TaskData> taskDataStorage;

        [SerializeField, ReadOnly] private ActiveTaskData activeTaskData;

        [SerializeReference] private IStorage<ActiveTaskData> activeTaskDataStorage;

        public IStorage<TaskData> TaskDataStorage => taskDataStorage;

        /// <summary>
        /// Gets the currently active task information.
        /// </summary>
        public ActiveTaskData ActiveTaskData => activeTaskData;

        private void Awake()
        {
            // When the component starts, load the active task data from storage if it exists.
            var optionalActiveTaskData = activeTaskDataStorage.LoadItem(GetUserID());

            // If the data exists, use it; otherwise, create a new ActiveTaskData instance with the user's unique ID.
            activeTaskData = optionalActiveTaskData.Unwrap(out var activeData)
                ? activeData
                : new ActiveTaskData(GetUserID());

            // Bind to the OnActiveTaskChanged event to save the active task data whenever it changes.
            activeTaskData.OnActiveTaskChanged += SaveActiveTaskData;
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void Start()
        {
            // As a test, save the active task data to storage when the component starts.
            SaveActiveTaskData(activeTaskData);
        }

        public void SetActiveTask(GUID uniqueID, Optional<float> setWorkingTime)
        {
            // Set the currently active task information to the task with the specified ID and working time.
            activeTaskData.SetActiveTask(uniqueID, true);

            // If a working time is provided, set it on the active task information.
            if (setWorkingTime.Unwrap(out var workingTime))
                activeTaskData.SetWorkingTime(workingTime);
        }

        private void SaveActiveTaskData(ActiveTaskData activeData)
        {
            // Save the active task data to storage whenever it changes.
            activeTaskDataStorage.SaveItem(activeData);
        }

        /// <summary>
        /// A helper function to get the unique user ID for the player.
        /// Since this is a single-player game, we can just return an empty GUID for now.
        /// </summary>
        /// <returns></returns>
        private GUID GetUserID()
        {
            return new GUID();
        }
    }
}