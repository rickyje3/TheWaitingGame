using System;
using System.Collections.Generic;
using Alchemy.Serialization;
using Tasks.Creation;
using Utilities.SaveLoad;

namespace Tasks.Storage
{
    /// <summary>
    /// An actual implementation of the ITaskDataStorage interface that uses JSON serialization to store task data.
    /// This class is responsible for saving, loading, and removing task data from a JSON file on disk.
    /// It uses a backing dictionary to avoid constantly reading and writing to the JSON file, which can be inefficient.
    /// The dictionary is serialized to JSON when the application is closed or when the data is explicitly saved,
    /// and it is deserialized from JSON when the application starts or when the data is explicitly loaded.
    /// </summary>
    [Serializable]
    public class JsonTaskDataStorage : JsonStorage<TaskData, TaskDataArray>
    {
        protected override TaskDataArray CreateItemArray(IEnumerable<TaskData> items)
        {
            return new TaskDataArray(items);
        }
    }
}