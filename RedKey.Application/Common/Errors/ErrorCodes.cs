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
