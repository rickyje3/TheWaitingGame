using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tasks.ActiveTask
{
    [Serializable]
    public class ActiveTaskDataArray : IEnumerable<ActiveTaskData>
    {
        [SerializeField] private ActiveTaskData[] taskDataArray;

        public ActiveTaskDataArray(IEnumerable<ActiveTaskData> items)
        {
            taskDataArray = items.ToArray();
        }

        #region IEnumerable Implementation

        public IEnumerator<ActiveTaskData> GetEnumerator()
        {
            return ((IEnumerable<ActiveTaskData>)taskDataArray).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ActiveTaskData>)taskDataArray).GetEnumerator();
        }

        #endregion
    }
}