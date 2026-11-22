using System;
using Alchemy.Inspector;
using Alchemy.Serialization;
using Tasks.ActiveTask;
using Tasks.Storage;
using UnityEngine;
using Utilities.Monads;
using Utilities.SaveLoad;

namespace Tasks
{
    [AlchemySerialize]
    public partial class TaskDataManager : MonoBehaviour
    {
        /// <summary>
        /// The data storage that holds the task data to be displayed in the list.
        /// This is NOT an optional field, as the TaskList requires a valid data storage to function correctly.
        /// </summary>
        [SerializeReference] private IStorage<TaskData.TaskData> taskDataStorage;

        [AlchemySerializeField, NonSerialized, ReadOnly]
        private ActiveTaskData _activeTaskData;

        [AlchemySerializeField, NonSerialized, ReadOnly]
        private IStorage<ActiveTaskData> _activeTaskDataStorage;

        public IStorage<TaskData.TaskData> TaskDataStorage => taskDataStorage;

        /// <summary>
        /// Gets the currently active task information.
        /// </summary>
        public ActiveTaskData ActiveTaskData => _activeTaskData;

        public void Awake()
        {
            // Create the active task data storage
            _activeTaskDataStorage = new ActiveTaskDataJsonStorage();

            // Bind to the onActiveTaskChanged event to save the active task data whenever it changes.
            _activeTaskData.onActiveTaskChanged += SaveActiveTaskData;

            // Initialize the active task information to None when the component is first created.
            _activeTaskData = new ActiveTaskData(GetUserID());
        }

        public void SetActiveTask(GUID uniqueID, Optional<float> setWorkingTime)
        {
            // Set the currently active task information to the task with the specified ID and working time.
            _activeTaskData.SetActiveTask(uniqueID);

            // If a working time is provided, set it on the active task information.
            if (setWorkingTime.Unwrap(out var workingTime))
                _activeTaskData.SetWorkingTime(workingTime);
        }

        private void SaveActiveTaskData(ActiveTaskData activeTaskData)
        {
            // Save the active task data to storage whenever it changes.
            _activeTaskDataStorage.SaveItem(activeTaskData);
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