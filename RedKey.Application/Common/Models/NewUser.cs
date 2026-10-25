using RedKey.Domain.Enums;

namespace RedKey.Application.Common.Models;

public sealed record NewUser(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password,
    UserRole Role,
    string PreferredLanguage);
