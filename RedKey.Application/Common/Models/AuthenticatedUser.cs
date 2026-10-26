using RedKey.Domain.Enums;

namespace RedKey.Application.Common.Models;

public sealed record AuthenticatedUser(
    Guid UserId,
    string FullName,
    string Email,
    string? PhoneNumber,
    UserRole Role,
    string PreferredLanguage);
