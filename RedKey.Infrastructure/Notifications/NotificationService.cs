using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RedKey.Application.Contracts.Notifications;
using RedKey.Application.Dtos.Notifications;
using RedKey.Domain.Common;
using RedKey.Domain.Entities;
using RedKey.Persistence.Contexts;

namespace RedKey.Infrastructure.Notifications;

/// <summary>Stores the notification (RK_Notifications) then pushes it to the recipients' open tabs.</summary>
internal sealed class NotificationService : INotificationService
{
    private readonly RedKeyDbContext _context;
    private readonly IHubContext<NotificationsHub> _hub;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(RedKeyDbContext context, IHubContext<NotificationsHub> hub, ILogger<NotificationService> logger)
    {
        _context = context;
        _hub = hub;
        _logger = logger;
    }

    public async Task NotifySuperAdminsAsync(NotificationMessage message, CancellationToken cancellationToken = default) =>
        await SendAsync(await SuperAdminIdsAsync(cancellationToken), message, cancellationToken);

    public async Task PushToSuperAdminsAsync(string eventName, object payload, CancellationToken cancellationToken = default)
    {
        var adminIds = await SuperAdminIdsAsync(cancellationToken);
        if (adminIds.Count == 0)
            return;

        try
        {
            await _hub.Clients.Users(adminIds.Select(id => id.ToString())).SendAsync(eventName, payload, cancellationToken);
        }
        catch (Exception ex)
        {
            // Stored by the caller anyway; the admins see it on the next refresh.
            _logger.LogWarning(ex, "Live push {Event} to the super admins failed", eventName);
        }
    }

    private Task<List<Guid>> SuperAdminIdsAsync(CancellationToken cancellationToken) =>
        (from userRole in _context.UserRoles
         join role in _context.Roles on userRole.RoleId equals role.Id
         where role.Name == RoleNames.SuperAdmin
         select userRole.UserId)
        .ToListAsync(cancellationToken);

    public Task NotifyUserAsync(Guid userId, NotificationMessage message, CancellationToken cancellationToken = default) =>
        SendAsync([userId], message, cancellationToken);

    private async Task SendAsync(IReadOnlyCollection<Guid> userIds, NotificationMessage message, CancellationToken cancellationToken)
    {
        if (userIds.Count == 0)
            return;

        // Each recipient gets the text in his own session language.
        var languages = await _context.Users.AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.PreferredLanguage, cancellationToken);

        var notifications = userIds
            .Select(id => Notification.Create(
                id, message.Type, message.TitleAr, message.TitleEn, message.BodyAr, message.BodyEn, message.LinkUrl, message.EntityId))
            .ToList();

        _context.Notifications.AddRange(notifications);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            var ar = !languages.TryGetValue(notification.RecipientUserId, out var lang) || lang != "en";
            var dto = new NotificationDto
            {
                Id = notification.Id,
                Type = (int)notification.Type,
                Title = ar ? notification.TitleAr : notification.TitleEn,
                Body = ar ? notification.BodyAr : notification.BodyEn,
                LinkUrl = notification.LinkUrl,
                IsRead = false,
                CreatedAtUtc = notification.CreatedAtUtc
            };

            try
            {
                await _hub.Clients.User(notification.RecipientUserId.ToString())
                    .SendAsync(NotificationsHub.NotificationEvent, dto, cancellationToken);
            }
            catch (Exception ex)
            {
                // The notification is stored anyway; the bell loads it on the next refresh.
                _logger.LogWarning(ex, "Real-time push failed for notification {NotificationId}.", notification.Id);
            }
        }
    }
}
