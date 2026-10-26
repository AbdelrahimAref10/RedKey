using RedKey.Domain.Enums;

namespace RedKey.Application.Contracts.Identity;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    UserRole? Role { get; }
    string? FullName { get; }
    bool IsAuthenticated { get; }
}
