using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Models;
using TaskManagementApi.Services;

namespace TaskManagementApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TasksController(ITaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskItem>>> GetTasks(
        CancellationToken cancellationToken)
    {
        var tasks = await taskService.GetTasksAsync(cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> GetTask(
        int id,
        CancellationToken cancellationToken)
    {
        var task = await taskService.GetTaskAsync(id, cancellationToken);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask(
        [FromBody] TaskItem task,
        CancellationToken cancellationToken)
    {
        var createdTask = await taskService.CreateTaskAsync(task, cancellationToken);

        return CreatedAtAction(
            nameof(GetTask),
            new { id = createdTask.Id },
            createdTask);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTask(
        int id,
        [FromBody] TaskItem updatedTask,
        CancellationToken cancellationToken)
    {
        var updated = await taskService.UpdateTaskAsync(id, updatedTask, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await taskService.DeleteTaskAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
