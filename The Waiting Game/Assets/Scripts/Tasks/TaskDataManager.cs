using System;
using Alchemy.Inspector;
using Alchemy.Serialization;
using Tasks.ActiveTask;
using Tasks.Storage;
using UnityEngine;

namespace Tasks
{
    [AlchemySerialize]
    public partial class TaskDataManager : MonoBehaviour
    {
        /// <summary>
        /// The data storage that holds the task data to be displayed in the list.
        /// This is NOT an optional field, as the TaskList requires a valid data storage to function correctly.
        /// </summary>
        [SerializeReference] private ITaskDataStorage taskDataStorage;

        [AlchemySerializeField, NonSerialized, ReadOnly]
        private ActiveTaskInformation _activeTaskInformation;

        public ITaskDataStorage TaskDataStorage => taskDataStorage;

        /// <summary>
        /// Gets the currently active task information.
        /// </summary>
        public ActiveTaskInformation ActiveTaskInformation => _activeTaskInformation;

        public void Awake()
        {
            // Initialize the active task information to None when the component is first created.
            _activeTaskInformation = new ActiveTaskInformation();
        }

        public void SetActiveTask(Guid uniqueID, Optional<float> setWorkingTime)
        {
            // Set the currently active task information to the task with the specified ID and working time.
            _activeTaskInformation.SetActiveTask(uniqueID);

            // If a working time is provided, set it on the active task information.
            if (setWorkingTime.Unwrap(out var workingTime))
                _activeTaskInformation.SetWorkingTime(workingTime);
        }
    }
}