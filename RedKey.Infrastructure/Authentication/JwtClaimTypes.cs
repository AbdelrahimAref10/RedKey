namespace RedKey.Infrastructure.Authentication;

internal static class JwtClaimTypes
{
    public const string Role = "role";
    public const string Name = "name";
    /// <summary>Session language (ar / en) of admins and brokers.</summary>
    public const string Language = "lang";
}
