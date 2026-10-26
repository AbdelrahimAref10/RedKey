using RedKey.Application.Common.Models;

namespace RedKey.Application.Dtos.Notifications;

public sealed class NotificationDto
{
    public required Guid Id { get; init; }
    public required int Type { get; init; }
    public required string Title { get; init; }
    public required string Body { get; init; }
    /// <summary>Front-end route to open when clicked.</summary>
    public required string LinkUrl { get; init; }
    public required bool IsRead { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}

public sealed class NotificationsPageDto
{
    public required PagedResult<NotificationDto> Page { get; init; }
    public required int UnreadCount { get; init; }
}
