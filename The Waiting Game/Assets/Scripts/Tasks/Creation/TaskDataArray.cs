using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tasks.Creation
{
    [Serializable]
    public struct TaskDataArray : IEnumerable<TaskData.TaskData>
    {
        [SerializeField] private TaskData.TaskData[] taskDataArray;

        public TaskDataArray(IEnumerable<TaskData.TaskData> taskDataList)
        {
            // Copy the task data from the list to the array.
            taskDataArray = taskDataList.ToArray();
        }

        #region IEnumerable Implementation

        public IEnumerator<TaskData.TaskData> GetEnumerator()
        {
            return ((IEnumerable<TaskData.TaskData>)taskDataArray).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TaskData.TaskData>)taskDataArray).GetEnumerator();
        }

        #endregion
    }
}