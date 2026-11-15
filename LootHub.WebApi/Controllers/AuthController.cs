using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace LootHub.WebApi.Controllers;

[Route("api/auth")]
public class AuthController(IAuthService authService) : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request, ct);

        return result.IsSuccess
            ? Ok(result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request, ct);

        return result.IsSuccess
            ? Ok(result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }
}
