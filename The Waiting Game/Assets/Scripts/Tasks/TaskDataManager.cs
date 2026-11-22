using UnityEngine;

namespace Tasks
{
    public class TaskDataManager : MonoBehaviour
    {
        /// <summary>
        /// The data storage that holds the task data to be displayed in the list.
        /// This is NOT an optional field, as the TaskList requires a valid data storage to function correctly.
        /// </summary>
        [SerializeReference] private ITaskDataStorage taskDataStorage;

        public ITaskDataStorage TaskDataStorage => taskDataStorage;
    }
}