using FluentValidation;
using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Auth;
using LootHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LootHub.Application.Features;

public class AuthService(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IValidator<LoginRequest> loginValidator,
    IValidator<RegisterRequest> registerValidator
    ) : IAuthService
{
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var validationResult = await loginValidator.ValidateAsync(request,ct);
        if (!validationResult.IsValid)
            return Result<AuthResponse>.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result<AuthResponse>.Failure(ErrorType.Unauthorized, "Invalid email or password.");
        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return Result<AuthResponse>.Failure(ErrorType.Unauthorized, "Invalid email or password.");

        var roles = await userManager.GetRolesAsync(user);
        var token = tokenService.CreateToken(user, roles);
        return Result<AuthResponse>.Success(ToResponse(user, token, roles));
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {

        request = request with
        {
            Email = request.Email.Trim(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim()
        };
        var validationResult = await registerValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result<AuthResponse>.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var userWithEmail = await userManager.FindByEmailAsync(request.Email);
        if (userWithEmail is not null)
            return Result<AuthResponse>.Failure(ErrorType.Conflict, "An account with this email already exists.");

        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            UserName = request.Email,
            Email = request.Email
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return Result<AuthResponse>.Failure(ErrorType.Validation, string.Join(", ", createResult.Errors.Select(e => e.Description)));

        var roles = await userManager.GetRolesAsync(user);
        var token = tokenService.CreateToken(user, roles);
        return Result<AuthResponse>.Success(ToResponse(user, token, roles));
    }

    private static AuthResponse ToResponse(AppUser user, AccessToken token, IList<string> roles) =>
    new(token.Token, token.ExpiresAt, user.Id, user.Email!.Trim(), user.FirstName, user.LastName, roles.ToList());
}
