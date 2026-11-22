using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;
using Alchemy.Serialization;
using Tasks.Storage;

namespace Tasks.Test
{
    [Serializable, AlchemySerialize]
    public partial class TaskDataStorageTest : ITaskDataStorage
    {
        /// <summary>
        /// The backing dictionary that stores the task data.
        /// In an actual implementation of the ITaskDataStorage interface,
        /// this would likely be replaced with a more robust storage solution (e.g., a database or file system).
        /// </summary>
        [AlchemySerializeField, NonSerialized, ReadOnly]
        private Dictionary<GUID, TaskData.TaskData> taskDataDictionary = new();

        #region ITaskDataStorage Implementation

        public Optional<TaskData.TaskData> LoadItem(GUID uniqueID)
        {
            // Check if the item exists in the dictionary.
            // If it does, return the item wrapped in an Optional.
            if (taskDataDictionary.TryGetValue(uniqueID, out var taskData))
                return taskData;

            // If the item does not exist, return an empty Optional
            return Optional<TaskData.TaskData>.None;
        }

        public bool SaveItem(TaskData.TaskData item)
        {
            // Check if the item already exists in the dictionary.
            // If it does, update the existing item.
            if (taskDataDictionary.ContainsKey(item.UniqueID))
            {
                taskDataDictionary[item.UniqueID] = item;
                return true; // Indicate that the item was updated.
            }

            // If the item does not exist, add it to the dictionary.
            taskDataDictionary.Add(item.UniqueID, item);
            return true; // Indicate that the item was added.
        }

        public Optional<TaskData.TaskData> RemoveItem(GUID uniqueID)
        {
            // Check if the item exists in the dictionary.
            // If it does, remove it and return the removed item wrapped in an Optional.
            if (taskDataDictionary.Remove(uniqueID, out var taskData))
                return taskData;

            // If the item does not exist, return an empty Optional.
            return Optional<TaskData.TaskData>.None;
        }

        public List<GUID> GetAllStoredItemIDs()
        {
            // Return a list of all the unique IDs of the stored items.
            return new List<GUID>(taskDataDictionary.Keys);
        }

        public List<TaskData.TaskData> GetAllStoredItems()
        {
            // Return a list of all the stored items.
            return new List<TaskData.TaskData>(taskDataDictionary.Values);
        }

        #endregion
    }
}