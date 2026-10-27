using TaskManagementApi.Models;

namespace TaskManagementApi.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskItem>> GetTasksAsync(CancellationToken cancellationToken = default);

    Task<TaskItem?> GetTaskAsync(int id, CancellationToken cancellationToken = default);

    Task<TaskItem> CreateTaskAsync(TaskItem task, CancellationToken cancellationToken = default);

    Task<bool> UpdateTaskAsync(int id, TaskItem updatedTask, CancellationToken cancellationToken = default);

    Task<bool> DeleteTaskAsync(int id, CancellationToken cancellationToken = default);
}
