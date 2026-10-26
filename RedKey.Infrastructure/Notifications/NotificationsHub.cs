using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RedKey.Infrastructure.Notifications;

/// <summary>
/// Real-time channel of the notifications. Clients only listen to <see cref="NotificationEvent"/>;
/// users are addressed by their id (the JWT <c>sub</c> claim).
/// </summary>
[Authorize]
public sealed class NotificationsHub : Hub
{
    public const string Route = "/hubs/notifications";
    public const string NotificationEvent = "notification";
}

internal sealed class SubClaimUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User?.FindFirst("sub")?.Value;
}
