using RedKey.Domain.Enums;

namespace RedKey.Domain.Common;

public static class RoleNames
{
    public const string SuperAdmin = nameof(UserRole.SuperAdmin);
    public const string Broker = nameof(UserRole.Broker);
    public const string Customer = nameof(UserRole.Customer);

    public static readonly IReadOnlyList<string> All = [SuperAdmin, Broker, Customer];

    public static string From(UserRole role) => role switch
    {
        UserRole.SuperAdmin => SuperAdmin,
        UserRole.Broker => Broker,
        UserRole.Customer => Customer,
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
    };

    public static UserRole? ToRole(string? roleName) => roleName switch
    {
        SuperAdmin => UserRole.SuperAdmin,
        Broker => UserRole.Broker,
        Customer => UserRole.Customer,
        _ => null
    };
}
