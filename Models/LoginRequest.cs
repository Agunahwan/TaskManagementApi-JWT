
using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Models;

public record LoginRequest(
    [Required]
    [StringLength(100, MinimumLength = 1)]
    string Username,

    [Required]
    [StringLength(100, MinimumLength = 1)]
    string Password
);
