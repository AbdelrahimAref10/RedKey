using RedKey.Application.Common.Models;

namespace RedKey.Application.Contracts.Identity;

public interface IIdentityService
{
    Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> IsPhoneTakenAsync(string phoneNumber, CancellationToken cancellationToken = default);

    Task<Result<AuthenticatedUser>> CreateUserAsync(NewUser user, CancellationToken cancellationToken = default);

    /// <summary>Validates the credentials of a user identified by email or phone number.</summary>
    Task<Result<AuthenticatedUser>> SignInAsync(string identifier, string password, CancellationToken cancellationToken = default);
}
