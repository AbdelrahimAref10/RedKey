using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.Notifications.MarkNotificationsRead;

/// <summary>Marks one notification (<paramref name="Id"/>) or all of the user's notifications (null) as read.</summary>
public sealed record MarkNotificationsReadCommand(Guid? Id) : IRequest<Result>;
