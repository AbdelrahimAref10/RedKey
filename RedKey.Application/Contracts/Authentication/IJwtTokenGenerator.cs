using RedKey.Application.Common.Models;

namespace RedKey.Application.Contracts.Authentication;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) Generate(AuthenticatedUser user);
}
