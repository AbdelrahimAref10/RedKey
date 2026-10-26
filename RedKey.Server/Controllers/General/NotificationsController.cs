using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Dtos.Notifications;
using RedKey.Application.Features.Notifications.GetMyNotifications;
using RedKey.Application.Features.Notifications.MarkNotificationsRead;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.General;

/// <summary>Notifications of the signed-in user (any role). Live updates come through the SignalR hub.</summary>
[ApiController]
[Authorize]
[Route("api/notifications")]
[Produces("application/json")]
public sealed class NotificationsController : ControllerBase
{
    private readonly ISender _sender;

    public NotificationsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Latest notifications; customers (web + mobile) send <c>lang</c>, dashboards use the session language.</summary>
    [HttpGet]
    [ProducesResponseType<NotificationsPageDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMine([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? lang, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetMyNotificationsQuery(page, pageSize), cancellationToken)).ToActionResult();

    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken) =>
        (await _sender.Send(new MarkNotificationsReadCommand(id), cancellationToken)).ToActionResult();

    [HttpPost("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken) =>
        (await _sender.Send(new MarkNotificationsReadCommand(null), cancellationToken)).ToActionResult();
}
