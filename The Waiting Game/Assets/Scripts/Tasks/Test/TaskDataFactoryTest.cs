using System;
using UnityEngine;

namespace Tasks.Test
{
    [Serializable]
    public class TaskDataFactoryTest : ITaskDataFactory
    {
        #region Task Creation Fields

        [SerializeField] private string taskTitle = "Default Task Title";
        [SerializeField] private string taskDescription = "Default Task Description";

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
            return GUID.Generate();
        }
    }
}