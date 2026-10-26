using RedKey.Application.Common.Models;

namespace RedKey.Application.Dtos.ContactMessages;

/// <summary>A message in the admin inbox list (and the live push).</summary>
public class ContactMessageListItemDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string CountryIso { get; init; }
    /// <summary>Without "+", e.g. 20.</summary>
    public required string DialCode { get; init; }
    /// <summary>National number without the leading 0 — WhatsApp: wa.me/{DialCode}{PhoneNumber}.</summary>
    public required string PhoneNumber { get; init; }
    public required int Subject { get; init; }
    public required string SubjectName { get; init; }
    /// <summary>The first characters of the message.</summary>
    public required string Preview { get; init; }
    public required bool IsRead { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}

/// <summary>The whole message (opening it marks it read).</summary>
public sealed class ContactMessageDetailsDto : ContactMessageListItemDto
{
    public required string Message { get; init; }
    public required string SenderEmail { get; init; }
    public DateTime? ReadAtUtc { get; init; }
}

/// <summary>Inbox page + how many are still unread (the header chat badge).</summary>
public sealed class ContactMessagesPageDto
{
    public required PagedResult<ContactMessageListItemDto> Page { get; init; }
    public required int UnreadCount { get; init; }
}
