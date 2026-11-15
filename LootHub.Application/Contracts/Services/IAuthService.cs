using LootHub.Application.Common;
using LootHub.Application.DTOs.Auth;

namespace LootHub.Application.Contracts.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
