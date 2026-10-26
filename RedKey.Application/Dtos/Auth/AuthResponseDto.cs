using RedKey.Domain.Enums;

namespace RedKey.Application.Dtos.Auth;

public sealed class AuthResponseDto
{
    public required string AccessToken { get; init; }
    public required DateTime ExpiresAtUtc { get; init; }
    public required AuthUserDto User { get; init; }
}

public sealed class AuthUserDto
{
    public required Guid Id { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public required UserRole Role { get; init; }
    public required string PreferredLanguage { get; init; }
}
