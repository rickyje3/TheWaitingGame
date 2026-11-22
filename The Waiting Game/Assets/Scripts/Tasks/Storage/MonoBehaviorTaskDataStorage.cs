using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Monads;
using Utilities.SaveLoad;

namespace Tasks.Storage
{
    /// <summary>
    /// A task data storage that uses a MonoBehaviour to hold the actual data storage implementation.
    /// This allows multiple instances of the data storage to point to the same underlying data storage,
    /// ensuring that all instances share the same data.
    /// </summary>
    [Serializable]
    public class MonoBehaviorTaskDataStorage : IStorage<TaskData>
    {
        /// <summary>
        /// The TaskDataManager that holds the actual data storage implementation.
        /// </summary>
        [SerializeField] private TaskDataManager taskDataManager;

        #region ITaskDataStorage Implementation

        public Optional<TaskData> LoadItem(GUID uniqueID)
        {
            return taskDataManager.TaskDataStorage.LoadItem(uniqueID);
        }

        public bool SaveItem(TaskData item)
        {
            return taskDataManager.TaskDataStorage.SaveItem(item);
        }

        public Optional<TaskData> RemoveItem(GUID uniqueID)
        {
            return taskDataManager.TaskDataStorage.RemoveItem(uniqueID);
        }

        public List<GUID> GetAllStoredItemIDs()
        {
            return taskDataManager.TaskDataStorage.GetAllStoredItemIDs();
        }

        public List<TaskData> GetAllStoredItems()
        {
            return taskDataManager.TaskDataStorage.GetAllStoredItems();
        }

        #endregion
    }
}