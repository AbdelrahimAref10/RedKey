namespace RedKey.Application.Features.ContactMessages.Common;

/// <summary>Phone normalisation shared by the handler and the validator.</summary>
public static class PhoneDigits
{
    public static string DialCode(string value) => new(value.Where(char.IsAsciiDigit).ToArray());

    /// <summary>Digits only, without the trunk prefix 0 (01012345678 → 1012345678).</summary>
    public static string National(string value) => new string(value.Where(char.IsAsciiDigit).ToArray()).TrimStart('0');
}
