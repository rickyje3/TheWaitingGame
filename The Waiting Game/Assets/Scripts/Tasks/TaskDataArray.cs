using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tasks
{
    [Serializable]
    public struct TaskDataArray
    {
        [SerializeField] public TaskData[] taskDataArray;

        public TaskDataArray(IEnumerable<TaskData> taskDataList)
        {
            // Copy the task data from the list to the array.
            taskDataArray = taskDataList.ToArray();
        }
    }
}