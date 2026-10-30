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
    public const string Cities = "RK_Cities";
    public const string Areas = "RK_Areas";
    public const string UnitTypes = "RK_UnitTypes";
    public const string Units = "RK_Units";
    public const string UnitDetails = "RK_UnitDetails";
    public const string UnitImages = "RK_UnitImages";
    public const string UnitPaymentPlans = "RK_UnitPaymentPlans";
    public const string UnitFeatures = "RK_UnitFeatures";
    public const string ViewingRequests = "RK_ViewingRequests";
    public const string Notifications = "RK_Notifications";
    public const string ContactMessages = "RK_ContactMessages";
    public const string Favorites = "RK_Favorites";

    /// <summary>Sequence behind the unit codes (RK-1000, RK-1001...).</summary>
    public const string UnitCodeSequence = "RK_UnitCodeSequence";

    public const string Accounts = "RK_Accounts";
    public const string JournalEntries = "RK_JournalEntries";
    public const string JournalLines = "RK_JournalLines";
    /// <summary>Sequence behind the journal entry numbers (JE-100001...).</summary>
    public const string JournalNumberSequence = "RK_JournalNumberSequence";
}
