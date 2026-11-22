using System;

namespace Tasks
{
    [Serializable]
    public class ActiveTaskInformation
    {
        /// <summary>
        /// The ID of the currently active task.
        /// </summary>
        private readonly Guid _uniqueID;

        /// <summary>
        /// The amount of time spent working while the currently active task is active.
        /// This value is updated in real-time as the task is being worked on.
        /// </summary>
        /// <note>This may or may not do anything at all</note>
        public float WorkingTime { get; private set; }

        public ActiveTaskInformation(Guid uniqueID)
        {
            _uniqueID = uniqueID;
        }

        public float AddWorkingTime(float timeToAdd)
        {
            return SetWorkingTime(WorkingTime + timeToAdd);
        }

        public float ResetWorkingTime()
        {
            return SetWorkingTime(0f);
        }

        public float SetWorkingTime(float newWorkingTime)
        {
            WorkingTime = newWorkingTime;
            return WorkingTime;
        }
    }
}