using System;
using System.ComponentModel;
using Alchemy.Inspector;
using Tasks.Test;
using UnityEngine;

namespace Tasks.TaskUI
{
    public class TaskList : MonoBehaviour
    {
        /// <summary>
        /// The prefab for the TaskListItem UI element that will be instantiated for each task in the list.
        /// </summary>
        [SerializeField] private TaskListItem taskListItemPrefab;

        /// <summary>
        /// The transform under which the TaskListItem UI elements will be instantiated.
        /// This is typically a UI container such as a Vertical Layout Group or a Scroll View content
        /// </summary>
        [SerializeField] private Transform taskListContainer;

        /// <summary>
        /// The data storage that holds the task data to be displayed in the list.
        /// This is NOT an optional field, as the TaskList requires a valid data storage to function correctly.
        /// </summary>
        [SerializeReference] private ITaskDataStorage taskDataStorage;

        private void Start()
        {
            CreateTestTasks();
            PopulateTaskList();
        }

        private void CreateTestTasks()
        {
            // Clear the existing task data in the storage before creating new test tasks.
            var taskIDs = taskDataStorage.GetAllStoredItemIDs();
            foreach (var id in taskIDs)
                taskDataStorage.RemoveItem(id);

            // Then, create 10 new test tasks and store them in the task data storage.
            for (var i = 0; i < 10; i++)
            {
                var taskData = new TaskData(GUID.Generate())
                {
                    title = $"Task {i + 1}",
                    description = $"This is the description for Task {i + 1}."
                };

                taskDataStorage.SaveItem(taskData);
            }
        }
        /// <summary>
        /// TODO: REMOVE
        /// </summary>
        [Button, DisableInEditMode]
        private void CreateNewTasksAndRefreshList()
        {
            CreateTestTasks();
            PopulateTaskList();
        }

        [Button, DisableInEditMode]
        private void PopulateTaskList()
        {
            // Clear any existing TaskListItem UI elements in the container.
            ClearTaskList();

            // Get all the unique IDs of the stored task data items.
            var taskIDs = taskDataStorage.GetAllStoredItemIDs();

            // Iterate through each unique ID and create a TaskListItem for each corresponding TaskData.
            foreach (var taskID in taskIDs)
            {
                var optionalTaskData = taskDataStorage.LoadItem(taskID);

                // Only create a TaskListItem if the TaskData exists (i.e., HasValue is true).
                if (!optionalTaskData.Unwrap(out var taskData))
                    continue;

                // Instantiate a new TaskListItem UI element from the prefab.
                var taskListItem = Instantiate(taskListItemPrefab, taskListContainer);

                // Set the TaskData for the newly created TaskListItem.
                taskListItem.SetTaskData(taskDataStorage, taskData);
            }
        }

        private void ClearTaskList()
        {
            // Get all the TaskListItem components that are children of the taskListContainer.
            var children = taskListContainer.GetComponentsInChildren<TaskListItem>();

            // Destroy all existing TaskListItem UI elements in the container.
            foreach (var child in children)
                Destroy(child.gameObject);
        }
    }
}