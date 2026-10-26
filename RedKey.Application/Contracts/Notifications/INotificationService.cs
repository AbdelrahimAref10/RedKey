using RedKey.Domain.Enums;

namespace RedKey.Application.Contracts.Notifications;

public sealed record NotificationMessage(
    NotificationType Type,
    string TitleAr,
    string TitleEn,
    string BodyAr,
    string BodyEn,
    string LinkUrl,
    Guid? EntityId);

/// <summary>Stores the notification and pushes it in real time (SignalR).</summary>
public interface INotificationService
{
    Task NotifySuperAdminsAsync(NotificationMessage message, CancellationToken cancellationToken = default);

    Task NotifyUserAsync(Guid userId, NotificationMessage message, CancellationToken cancellationToken = default);

    /// <summary>Live push only (nothing stored) of <paramref name="payload"/> as <paramref name="eventName"/> to the super admins.</summary>
    Task PushToSuperAdminsAsync(string eventName, object payload, CancellationToken cancellationToken = default);
}
