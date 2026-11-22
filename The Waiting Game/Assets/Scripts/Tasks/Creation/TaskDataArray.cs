using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tasks.Creation
{
    [Serializable]
    public struct TaskDataArray
    {
        [SerializeField] public TaskData.TaskData[] taskDataArray;

        public TaskDataArray(IEnumerable<TaskData.TaskData> taskDataList)
        {
            // Copy the task data from the list to the array.
            taskDataArray = taskDataList.ToArray();
        }
    }
}