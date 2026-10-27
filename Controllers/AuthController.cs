using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TaskManagementApi.Models;

namespace TaskManagementApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        // Demo-only credential check for this tutorial. Replace with a real user store
        // and a secure password-hashing strategy in production.
        var expectedUsername = configuration["Auth:Username"];
        var expectedPassword = configuration["Auth:Password"];

        if (string.IsNullOrWhiteSpace(expectedUsername) ||
            string.IsNullOrWhiteSpace(expectedPassword))
        {
            return Problem(
                title: "Authentication is not configured.",
                detail: "Set Auth:Username and Auth:Password using .NET User Secrets.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!string.Equals(request.Username, expectedUsername, StringComparison.Ordinal) ||
            !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        var signingKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT signing key is not configured.");
        var issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT issuer is not configured.");
        var audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT audience is not configured.");
        var tokenMinutes = configuration.GetValue<int>("Jwt:AccessTokenMinutes", 30);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(tokenMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Username),
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new LoginResponse(accessToken, "Bearer", expiresAtUtc));
    }
}
