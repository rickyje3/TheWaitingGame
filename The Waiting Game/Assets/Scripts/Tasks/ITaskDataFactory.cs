using Tasks;

/// <summary>
/// An interface that defines a factory for creating task data.
/// </summary>
public interface ITaskDataFactory
{
    /// <summary>
    /// Creates a new instance of TaskData.
    /// Implementations *should* have their own ways of setting the data within the TaskData struct.
    /// </summary>
    /// <returns>An Optional containing the created TaskData if successful; otherwise, an empty Optional.</returns>
    public Optional<TaskData> CreateTaskData();
}