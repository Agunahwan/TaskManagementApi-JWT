using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Models;

public sealed record LoginRequest(
    [property: Required, StringLength(100, MinimumLength = 1)] string Username,
    [property: Required, StringLength(200, MinimumLength = 1)] string Password);
