namespace RedKey.Application.Contracts.Localization;

/// <summary>
/// Response language: the explicit <c>?lang=</c> sent by the landing / customer clients (web + mobile),
/// otherwise the <c>lang</c> claim of the signed-in user (admin / broker session), otherwise Arabic.
/// </summary>
public interface ILanguageContext
{
    string Lang { get; }
    bool IsArabic { get; }
}
