using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.ViewingRequests;
using RedKey.Application.Features.ViewingRequests.GetViewingRequestsAdmin;
using RedKey.Application.Features.ViewingRequests.GetViewingRequestsSummary;
using RedKey.Application.Features.ViewingRequests.UpdateViewingRequestStatus;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.SuperAdmin;

/// <summary>The viewing requests customers send from the unit pages.</summary>
[Route("api/admin/viewing-requests")]
public sealed class AdminViewingRequestsController : SuperAdminControllerBase
{
    private readonly ISender _sender;

    public AdminViewingRequestsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Newest first; <c>status</c> from the lookups endpoint, <c>search</c> = name / phone / unit code.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResult<ViewingRequestAdminDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRequests(
        [FromQuery] int? status,
        [FromQuery] string? search,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken) =>
        (await _sender.Send(new GetViewingRequestsAdminQuery(status, search, page, pageSize), cancellationToken)).ToActionResult();

    /// <summary>Count per status (tabs, sidebar badge, dashboard).</summary>
    [HttpGet("summary")]
    [ProducesResponseType<ViewingRequestsSummaryDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken) =>
        (await _sender.Send(new GetViewingRequestsSummaryQuery(), cancellationToken)).ToActionResult();

    /// <summary>Moves the request on; the customer gets a notification.</summary>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateViewingRequestStatusCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command with { Id = id }, cancellationToken)).ToActionResult();
}
