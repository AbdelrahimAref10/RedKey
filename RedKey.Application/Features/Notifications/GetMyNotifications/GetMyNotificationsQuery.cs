using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Notifications;

namespace RedKey.Application.Features.Notifications.GetMyNotifications;

/// <summary>Notifications of the signed-in user (newest first) + the unread count.</summary>
public sealed record GetMyNotificationsQuery(int? Page, int? PageSize) : IRequest<Result<NotificationsPageDto>>;
