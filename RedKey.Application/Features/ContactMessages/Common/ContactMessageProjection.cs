using RedKey.Application.Common.Localization;
using RedKey.Application.Dtos.ContactMessages;
using RedKey.Domain.Entities;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.ContactMessages.Common;

internal static class ContactMessageProjection
{
    public const int PreviewLength = 140;

    /// <summary>The SQL part (the subject name is added in memory).</summary>
    public static IQueryable<ContactMessageRow> ToRows(this IQueryable<ContactMessage> messages) =>
        messages.Select(m => new ContactMessageRow(
            m.Id,
            m.Name,
            m.CountryIso,
            m.DialCode,
            m.PhoneNumber,
            m.Subject,
            m.Message.Length > PreviewLength ? m.Message.Substring(0, PreviewLength) : m.Message,
            m.IsRead,
            m.CreatedAtUtc));

    public static ContactMessageListItemDto ToListItem(this ContactMessageRow r, bool ar) => new()
    {
        Id = r.Id,
        Name = r.Name,
        CountryIso = r.CountryIso,
        DialCode = r.DialCode,
        PhoneNumber = r.PhoneNumber,
        Subject = (int)r.Subject,
        SubjectName = EnumNames.Of(r.Subject, ar),
        Preview = r.Preview,
        IsRead = r.IsRead,
        CreatedAtUtc = r.CreatedAtUtc
    };

    public static ContactMessageListItemDto ToListItem(this ContactMessage m, bool ar) =>
        new ContactMessageRow(
            m.Id, m.Name, m.CountryIso, m.DialCode, m.PhoneNumber, m.Subject,
            m.Message.Length > PreviewLength ? m.Message[..PreviewLength] : m.Message,
            m.IsRead, m.CreatedAtUtc).ToListItem(ar);
}

internal sealed record ContactMessageRow(
    Guid Id,
    string Name,
    string CountryIso,
    string DialCode,
    string PhoneNumber,
    ContactSubject Subject,
    string Preview,
    bool IsRead,
    DateTime CreatedAtUtc);
