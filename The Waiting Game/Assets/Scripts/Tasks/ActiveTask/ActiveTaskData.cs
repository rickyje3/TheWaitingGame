using System;
using Alchemy.Inspector;
using UnityEngine;
using Utilities.Monads;
using Utilities.SaveLoad;

namespace Tasks.ActiveTask
{
    /// <summary>
    /// A class that represents the data for an active task.
    /// Each active task is associated with a specific user and has a unique ID.
    /// However, this is a single player game, so the user ID is not used for anything.
    /// </summary>
    [Serializable]
    public class ActiveTaskData : IHasUniqueID
    {
        /// <summary>
        /// The ID of the user associated with the currently active task.
        /// </summary>
        [SerializeField, ReadOnly] private GUID userID;

        /// <summary>
        /// The ID of the currently active task.
        /// </summary>
        [SerializeField, ReadOnly] private Optional<GUID> taskID;

        /// <summary>
        /// The ID of the user associated with the currently active task.
        /// </summary>
        public GUID UniqueID => userID;

        /// <summary>
        /// The amount of time spent working while the currently active task is active.
        /// This value is updated in real-time as the task is being worked on.
        /// </summary>
        /// <note>This may or may not do anything at all</note>
        public float WorkingTime { get; private set; }

        public event Action<ActiveTaskData> onActiveTaskChanged;

        /// <summary>
        /// Creates a new instance of the ActiveTaskData class with the specified user ID.
        /// </summary>
        /// <param name="userId"></param>
        public ActiveTaskData(GUID userId)
        {
            this.userID = userId;
            RemoveActiveTask();
        }

        public ActiveTaskData(GUID userId, GUID taskId)
        {
            this.userID = userId;
            SetActiveTask(taskId);
        }

        /// <summary>
        /// Sets the currently active task to the task with the specified ID.
        /// </summary>
        /// <param name="taskId"></param>
        public void SetActiveTask(GUID taskId)
        {
            taskID = taskId;

            // Invoke the onActiveTaskChanged event to notify any listeners that the active task has changed.
            onActiveTaskChanged?.Invoke(this);
        }

        /// <summary>
        /// Removes the currently active task, setting the active task ID to None.
        /// </summary>
        public void RemoveActiveTask()
        {
            taskID = Optional<GUID>.None;

            // Invoke the onActiveTaskChanged event to notify any listeners that the active task has changed.
            onActiveTaskChanged?.Invoke(this);
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