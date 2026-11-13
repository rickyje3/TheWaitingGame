using System;
using UnityEngine;

namespace Tasks
{
    /// <summary>
    /// A struct that represents the data for a task.
    /// This struct can be used to store information about a task, such as its name, description, and any other relevant data.
    /// This will be used to pass data to different functions.
    /// </summary>
    [Serializable]
    public struct TaskData : IHasUniqueID
    {
        /// <summary>
        /// A unique ID to identify the task.
        /// This can be used to differentiate between different tasks, even if they have the same name or description.
        /// </summary>
        [SerializeField] public GUID uniqueID;

        /// <summary>
        /// The title of the task.
        /// This is a brief, descriptive name for the task that can be displayed to the player.
        /// </summary>
        [SerializeField] public string title;

        /// <summary>
        /// The description of the task (optional).
        /// This is a more detailed explanation of what the task entails, including any objectives or requirements/
        /// </summary>
        [SerializeField] public string description;

        // /// <summary>
        // /// The due date of the task.
        // /// This is the date by which the task should be completed.
        // /// </summary>
        // [SerializeField] public DateTime dueDate;
        public GUID UniqueID => uniqueID;
    }
}