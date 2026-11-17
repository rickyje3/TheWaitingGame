namespace Tasks
{
    public static class TaskDataLibrary
    {
        /// <summary>
        /// Creates a new TaskData instance using the provided factory and stores it in the provided storage.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="storage"></param>
        /// <returns></returns>
        public static Optional<TaskData> CreateAndStoreTaskData(ITaskDataFactory factory, ITaskDataStorage storage)
        {
            var taskData = factory.CreateTaskData();

            if (!taskData.HasValue)
                return Optional<TaskData>.None;

            storage.SaveItem(taskData.Value);
            return taskData;
        }
    }
}