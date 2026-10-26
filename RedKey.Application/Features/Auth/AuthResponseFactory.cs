using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Authentication;
using RedKey.Application.Dtos.Auth;

namespace RedKey.Application.Features.Auth;

internal static class AuthResponseFactory
{
    public static AuthResponseDto Create(AuthenticatedUser user, IJwtTokenGenerator tokenGenerator)
    {
        var (token, expiresAtUtc) = tokenGenerator.Generate(user);

        return new AuthResponseDto
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            User = new AuthUserDto
            {
                Id = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                PreferredLanguage = user.PreferredLanguage
            }
        };
    }
}
