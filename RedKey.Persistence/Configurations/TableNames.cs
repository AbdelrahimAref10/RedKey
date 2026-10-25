namespace RedKey.Persistence.Configurations;

/// <summary>Every table in the database is prefixed with <c>RK_</c>.</summary>
internal static class TableNames
{
    public const string Users = "RK_Users";
    public const string Roles = "RK_Roles";
    public const string UserRoles = "RK_UserRoles";
    public const string UserClaims = "RK_UserClaims";
    public const string UserLogins = "RK_UserLogins";
    public const string UserTokens = "RK_UserTokens";
    public const string RoleClaims = "RK_RoleClaims";
    public const string Customers = "RK_Customers";
    public const string Brokers = "RK_Brokers";
}
