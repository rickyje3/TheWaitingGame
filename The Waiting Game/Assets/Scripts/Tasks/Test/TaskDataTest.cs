using Alchemy.Inspector;
using Tasks.Creation;
using Tasks.Storage;
using UnityEngine;

namespace Tasks.Test
{
    public class TaskDataTest : MonoBehaviour
    {
        /// <summary>
        /// The factory used to create TaskData instances for testing purposes.
        /// </summary>
        [SerializeReference] private ITaskDataFactory taskDataFactory;

        /// <summary>
        /// The storage used to store TaskData instances for testing purposes.
        /// </summary>
        [SerializeReference] private ITaskDataStorage taskDataStorage;

        [Button, DisableInEditMode]
        public void CreateAndStoreTaskData()
        {
            var taskData = TaskDataLibrary.CreateAndStoreTaskData(taskDataFactory, taskDataStorage);

            switch (taskData.HasValue)
            {
                case true:
                    Debug.Log($"TaskData created and stored successfully: {taskData.Value.title}");
                    break;

                case false:
                    Debug.LogWarning("Failed to create and store TaskData.");
                    return;
            }
        }
    }
}