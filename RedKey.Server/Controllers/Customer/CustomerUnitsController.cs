using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Units;
using RedKey.Application.Features.Units.Common;
using RedKey.Application.Features.Units.GetUnitById;
using RedKey.Application.Features.ViewingRequests.CreateViewingRequest;
using RedKey.Server.Extensions;
using RedKey.Server.Models;

namespace RedKey.Server.Controllers.Customer;

/// <summary>
/// Units for the customer apps (web + mobile). Reading works without a session; the client always
/// sends <c>?lang=</c>. Requesting a viewing needs a signed-in customer.
/// </summary>
[Route("api/customer/units")]
public sealed class CustomerUnitsController : CustomerControllerBase
{
    private readonly ISender _sender;

    public CustomerUnitsController(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType<PagedResult<UnitListItemDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnits([FromQuery] PublicUnitsFilter filter, CancellationToken cancellationToken) =>
        (await _sender.Send(filter.ToQuery(UnitsScope.Public), cancellationToken)).ToActionResult();

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType<UnitDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUnit(Guid id, [FromQuery] string? lang, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetUnitByIdQuery(id, UnitsScope.Public), cancellationToken)).ToActionResult();

    /// <summary>Request a viewing of a published unit (signed-in customer).</summary>
    [HttpPost("{id:guid}/viewing-requests")]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequestViewing(Guid id, [FromBody] CreateViewingRequestCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command with { UnitId = id }, cancellationToken)).ToActionResult();
}
