using System;
using System.Collections.Generic;
using Utilities.SaveLoad;

namespace Tasks.ActiveTask
{
    /// <summary>
    /// A storage class for ActiveTaskData that uses JSON serialization to store the data.
    /// This stores active task data on a per-user basis, allowing each user to have their own active task data.
    /// However, this is a single-player game, so...
    /// </summary>
    [Serializable]
    public class ActiveTaskDataJsonStorage : JsonStorage<ActiveTaskData, ActiveTaskDataArray>
    {
        protected override ActiveTaskDataArray CreateItemArray(IEnumerable<ActiveTaskData> items)
        {
            return new ActiveTaskDataArray(items);
        }
    }
}