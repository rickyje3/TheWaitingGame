using System;
using Alchemy.Inspector;
using UnityEngine;

namespace Tasks.ActiveTask
{
    [Serializable]
    public class ActiveTaskData
    {
        /// <summary>
        /// The ID of the currently active task.
        /// </summary>
        [SerializeField, ReadOnly] private Optional<Guid> uniqueID;

        /// <summary>
        /// The amount of time spent working while the currently active task is active.
        /// This value is updated in real-time as the task is being worked on.
        /// </summary>
        /// <note>This may or may not do anything at all</note>
        public float WorkingTime { get; private set; }

        public ActiveTaskData()
        {
            RemoveActiveTask();
        }

        public ActiveTaskData(Guid id)
        {
            SetActiveTask(id);
        }

        /// <summary>
        /// Sets the currently active task to the task with the specified ID.
        /// </summary>
        /// <param name="id"></param>
        public void SetActiveTask(Guid id)
        {
            uniqueID = id;
        }

        /// <summary>
        /// Removes the currently active task, setting the active task ID to None.
        /// </summary>
        public void RemoveActiveTask()
        {
            uniqueID = Optional<Guid>.None;
        }


        public float SetWorkingTime(float newWorkingTime)
        {
            WorkingTime = newWorkingTime;
            return WorkingTime;
        }

        public float AddWorkingTime(float timeToAdd)
        {
            return SetWorkingTime(WorkingTime + timeToAdd);
        }

        public float ResetWorkingTime()
        {
            return SetWorkingTime(0f);
        }
    }
}