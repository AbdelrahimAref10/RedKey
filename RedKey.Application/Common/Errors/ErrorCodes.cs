namespace RedKey.Application.Common.Errors;

/// <summary>
/// Stable error codes returned to the client, which translates them (ar / en).
/// </summary>
public static class ErrorCodes
{
    public static class Auth
    {
        public const string InvalidCredentials = "auth.invalidCredentials";
        public const string LockedOut = "auth.lockedOut";
        public const string Inactive = "auth.inactive";
        public const string EmailTaken = "auth.emailTaken";
        public const string PhoneTaken = "auth.phoneTaken";
        public const string RoleNotAllowed = "auth.roleNotAllowed";
        public const string RegistrationFailed = "auth.registrationFailed";
    }

    public static class Units
    {
        public const string NotFound = "units.notFound";
        public const string Forbidden = "units.forbidden";
        public const string InvalidUnitType = "units.invalidUnitType";
        public const string InvalidArea = "units.invalidArea";
        public const string InvalidImage = "units.invalidImage";
        public const string ImageTooLarge = "units.imageTooLarge";
        public const string CoverRequired = "units.coverRequired";
        public const string TooManyImages = "units.tooManyImages";
        public const string NotAvailable = "units.notAvailable";
    }

    public static class Lookups
    {
        public const string UnitTypeNotFound = "lookups.unitTypeNotFound";
        public const string CityNotFound = "lookups.cityNotFound";
        public const string AreaNotFound = "lookups.areaNotFound";
    }

    public static class ViewingRequests
    {
        public const string NotFound = "viewingRequests.notFound";
    }

    public static class Accounting
    {
        public const string InvalidCashAccount = "accounting.invalidCashAccount";
    }

    public static class ContactMessages
    {
        public const string NotFound = "contactMessages.notFound";
    }

    public static class Notifications
    {
        public const string NotFound = "notifications.notFound";
    }

    public static class Validation
    {
        public const string Required = "validation.required";
        public const string InvalidEmail = "validation.invalidEmail";
        public const string InvalidPhone = "validation.invalidPhone";
        public const string MaxLength = "validation.maxLength";
        public const string PasswordWeak = "validation.passwordWeak";
        public const string PasswordMismatch = "validation.passwordMismatch";
        public const string InvalidValue = "validation.invalidValue";
    }
}
