using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}
