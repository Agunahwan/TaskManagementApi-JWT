using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services;

public class TaskService(AppDbContext dbContext) : ITaskService
{
    public async Task<IReadOnlyList<TaskItem>> GetTasksAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.TaskItems
            .AsNoTracking()
            .OrderBy(task => task.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetTaskAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.TaskItems
            .AsNoTracking()
            .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
    }

    public async Task<TaskItem> CreateTaskAsync(
        TaskItem task,
        CancellationToken cancellationToken = default)
    {
        // SQL Server generates the Id because it is an identity column.
        var newTask = new TaskItem
        {
            Title = task.Title,
            IsCompleted = task.IsCompleted
        };

        dbContext.TaskItems.Add(newTask);
        await dbContext.SaveChangesAsync(cancellationToken);

        return newTask;
    }

    public async Task<bool> UpdateTaskAsync(
        int id,
        TaskItem updatedTask,
        CancellationToken cancellationToken = default)
    {
        var task = await dbContext.TaskItems
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (task is null)
        {
            return false;
        }

        task.Title = updatedTask.Title;
        task.IsCompleted = updatedTask.IsCompleted;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteTaskAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var task = await dbContext.TaskItems
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (task is null)
        {
            return false;
        }

        dbContext.TaskItems.Remove(task);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
