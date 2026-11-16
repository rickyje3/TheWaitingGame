using System.Collections.Generic;
using UnityEngine;

namespace Tasks.Test
{
    public class TaskDataStorageTest : MonoBehaviour, ITaskDataStorage, ITaskDataFactory
    {
        /// <summary>
        /// The backing dictionary that stores the task data.
        /// In an actual implementation of the ITaskDataStorage interface,
        /// this would likely be replaced with a more robust storage solution (e.g., a database or file system).
        /// </summary>
        private readonly Dictionary<GUID, TaskData> _taskDataDictionary = new();

        #region Task Creation Fields

        [SerializeField] private string taskTitle = "Default Task Title";
        [SerializeField] private string taskDescription = "Default Task Description";

        #endregion

        #region ITaskDataStorage Implementation

        public Optional<TaskData> LoadItem(GUID uniqueID)
        {
            // Check if the item exists in the dictionary.
            // If it does, return the item wrapped in an Optional.
            if (_taskDataDictionary.TryGetValue(uniqueID, out var taskData))
                return taskData;

            // If the item does not exist, return an empty Optional
            return Optional<TaskData>.None;
        }

        public bool SaveItem(TaskData item)
        {
            // Check if the item already exists in the dictionary.
            // If it does, update the existing item.
            if (_taskDataDictionary.ContainsKey(item.UniqueID))
            {
                _taskDataDictionary[item.UniqueID] = item;
                return true; // Indicate that the item was updated.
            }

            // If the item does not exist, add it to the dictionary.
            _taskDataDictionary.Add(item.UniqueID, item);
            return true; // Indicate that the item was added.
        }

        public Optional<TaskData> RemoveItem(GUID uniqueID)
        {
            // Check if the item exists in the dictionary.
            // If it does, remove it and return the removed item wrapped in an Optional.
            if (_taskDataDictionary.Remove(uniqueID, out var taskData))
                return taskData;

            // If the item does not exist, return an empty Optional.
            return Optional<TaskData>.None;
        }

        public List<GUID> GetAllStoredItemIDs()
        {
            // Return a list of all the unique IDs of the stored items.
            return new List<GUID>(_taskDataDictionary.Keys);
        }

        public List<TaskData> GetAllStoredItems()
        {
            // Return a list of all the stored items.
            return new List<TaskData>(_taskDataDictionary.Values);
        }

        #endregion

        #region ITaskDataFactory Implementation

        public Optional<TaskData> CreateTaskData()
        {
            // Create a unique ID for the new task data.
            var uniqueID = GenerateUniqueID();

            var taskData = new TaskData(uniqueID)
            {
                title = taskTitle,
                description = taskDescription,
            };

            return taskData;
        }

        #endregion

        private GUID GenerateUniqueID()
        {
            return new GUID();
        }
    }
}