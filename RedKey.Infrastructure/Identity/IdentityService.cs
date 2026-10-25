using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Domain.Common;
using RedKey.Domain.Entities.Identity;

namespace RedKey.Infrastructure.Identity;

internal sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TimeProvider _timeProvider;

    public IdentityService(UserManager<ApplicationUser> userManager, TimeProvider timeProvider)
    {
        _userManager = userManager;
        _timeProvider = timeProvider;
    }

    public Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = _userManager.NormalizeEmail(email);
        return _userManager.Users.AnyAsync(u => u.NormalizedEmail == normalized, cancellationToken);
    }

    public Task<bool> IsPhoneTakenAsync(string phoneNumber, CancellationToken cancellationToken = default) =>
        _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

    public async Task<Result<AuthenticatedUser>> CreateUserAsync(NewUser newUser, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = newUser.Email,
            Email = newUser.Email,
            PhoneNumber = newUser.PhoneNumber,
            FullName = newUser.FullName,
            PreferredLanguage = newUser.PreferredLanguage,
            LastLoginAtUtc = _timeProvider.GetUtcNow().UtcDateTime
        };

        var created = await _userManager.CreateAsync(user, newUser.Password);
        if (!created.Succeeded)
            return Result<AuthenticatedUser>.Failure(MapIdentityError(created));

        var roleAdded = await _userManager.AddToRoleAsync(user, RoleNames.From(newUser.Role));
        if (!roleAdded.Succeeded)
            return Result<AuthenticatedUser>.Failure(ErrorCodes.Auth.RegistrationFailed);

        return Result<AuthenticatedUser>.Success(
            new AuthenticatedUser(user.Id, user.FullName, user.Email, user.PhoneNumber, newUser.Role));
    }

    public async Task<Result<AuthenticatedUser>> SignInAsync(
        string identifier,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = identifier.Contains('@')
            ? await _userManager.FindByEmailAsync(identifier)
            : await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == identifier, cancellationToken);

        if (user is null)
            return Unauthorized(ErrorCodes.Auth.InvalidCredentials);

        if (!user.IsActive)
            return Result<AuthenticatedUser>.Failure(ErrorCodes.Auth.Inactive, 403);

        if (await _userManager.IsLockedOutAsync(user))
            return Result<AuthenticatedUser>.Failure(ErrorCodes.Auth.LockedOut, 423);

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            await _userManager.AccessFailedAsync(user);
            return Unauthorized(ErrorCodes.Auth.InvalidCredentials);
        }

        var roleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        if (RoleNames.ToRole(roleName) is not { } role)
            return Unauthorized(ErrorCodes.Auth.InvalidCredentials);

        // Single UPDATE: resets the failed counter (when needed) and stamps the last login.
        user.LastLoginAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
        user.AccessFailedCount = 0;
        await _userManager.UpdateAsync(user);

        return Result<AuthenticatedUser>.Success(
            new AuthenticatedUser(user.Id, user.FullName, user.Email!, user.PhoneNumber, role));
    }

    private static Result<AuthenticatedUser> Unauthorized(string error) =>
        Result<AuthenticatedUser>.Failure(error, 401);

    private static string MapIdentityError(IdentityResult result)
    {
        var codes = result.Errors.Select(e => e.Code).ToHashSet();

        if (codes.Contains(nameof(IdentityErrorDescriber.DuplicateEmail)) ||
            codes.Contains(nameof(IdentityErrorDescriber.DuplicateUserName)))
            return ErrorCodes.Auth.EmailTaken;

        if (codes.Any(c => c.StartsWith("Password", StringComparison.Ordinal)))
            return ErrorCodes.Validation.PasswordWeak;

        if (codes.Contains(nameof(IdentityErrorDescriber.InvalidEmail)))
            return ErrorCodes.Validation.InvalidEmail;

        return ErrorCodes.Auth.RegistrationFailed;
    }
}
