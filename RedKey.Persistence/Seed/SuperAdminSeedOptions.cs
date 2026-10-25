namespace RedKey.Persistence.Seed;

public sealed class SuperAdminSeedOptions
{
    public const string SectionName = "Seed:SuperAdmin";

    public string FullName { get; init; } = "RedKey Super Admin";
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string Password { get; init; } = string.Empty;
}
