using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Dtos.ContactMessages;
using RedKey.Application.Features.ContactMessages.DeleteContactMessage;
using RedKey.Application.Features.ContactMessages.GetContactMessagesAdmin;
using RedKey.Application.Features.ContactMessages.OpenContactMessage;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.SuperAdmin;

/// <summary>The inbox of the website contact messages. New ones are pushed live on the hub ("contactMessage").</summary>
[Route("api/admin/contact-messages")]
public sealed class AdminContactMessagesController : SuperAdminControllerBase
{
    private readonly ISender _sender;

    public AdminContactMessagesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Newest first + the unread count. <c>search</c> = name, phone or text.</summary>
    [HttpGet]
    [ProducesResponseType<ContactMessagesPageDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessages(
        [FromQuery] bool? unreadOnly,
        [FromQuery] string? search,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken) =>
        (await _sender.Send(new GetContactMessagesAdminQuery(unreadOnly, search, page, pageSize), cancellationToken)).ToActionResult();

    /// <summary>The whole message; opening it marks it read.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<ContactMessageDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Open(Guid id, CancellationToken cancellationToken) =>
        (await _sender.Send(new OpenContactMessageCommand(id), cancellationToken)).ToActionResult();

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken) =>
        (await _sender.Send(new DeleteContactMessageCommand(id), cancellationToken)).ToActionResult();
}
