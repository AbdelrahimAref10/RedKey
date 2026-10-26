using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Units;
using RedKey.Application.Features.Units.Common;
using RedKey.Application.Features.Units.CreateUnit;
using RedKey.Application.Features.Units.DeleteUnit;
using RedKey.Application.Features.Units.GetUnitById;
using RedKey.Application.Features.Units.GetUnitForEdit;
using RedKey.Application.Features.Units.UpdateUnit;
using RedKey.Server.Extensions;
using RedKey.Server.Models;

namespace RedKey.Server.Controllers.Broker;

/// <summary>
/// Broker units: "my listings" (every status) + the published catalog with commissions.
/// Create / update use the same commands as the super admin; a broker unit always waits for review.
/// </summary>
[Route("api/broker/units")]
public sealed class BrokerUnitsController : BrokerControllerBase
{
    private const long MaxUnitRequestBytes = 32 * 1024 * 1024;
    private readonly ISender _sender;

    public BrokerUnitsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>The broker's own units (my listings), 10 per page.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResult<UnitListItemDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyUnits([FromQuery] ManageUnitsFilter filter, CancellationToken cancellationToken) =>
        (await _sender.Send(filter.ToQuery(UnitsScope.BrokerOwn), cancellationToken)).ToActionResult();

    /// <summary>Published units with their commission (available units to sell).</summary>
    [HttpGet("catalog")]
    [ProducesResponseType<PagedResult<UnitListItemDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalog([FromQuery] PublicUnitsFilter filter, CancellationToken cancellationToken) =>
        (await _sender.Send(filter.ToQuery(UnitsScope.BrokerCatalog), cancellationToken)).ToActionResult();

    [HttpGet("{id:guid}")]
    [ProducesResponseType<UnitDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUnit(Guid id, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetUnitByIdQuery(id, UnitsScope.BrokerOwn), cancellationToken)).ToActionResult();

    [HttpGet("{id:guid}/edit")]
    [ProducesResponseType<UnitEditDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUnitForEdit(Guid id, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetUnitForEditQuery(id, UnitsScope.BrokerOwn), cancellationToken)).ToActionResult();

    /// <summary>Unit types, areas and enums for the unit form.</summary>
    [HttpGet("form-lookups")]
    [ProducesResponseType<UnitFormLookupsDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFormLookups(CancellationToken cancellationToken) =>
        Ok(await _sender.GetUnitFormLookupsAsync(cancellationToken));

    /// <summary>Submit a unit for review (the super admins are notified).</summary>
    [HttpPost]
    [RequestSizeLimit(MaxUnitRequestBytes)]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUnitCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command, cancellationToken)).ToActionResult();

    /// <summary>Edit an own unit; it goes back to review.</summary>
    [HttpPut("{id:guid}")]
    [RequestSizeLimit(MaxUnitRequestBytes)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command with { Id = id }, cancellationToken)).ToActionResult();

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken) =>
        (await _sender.Send(new DeleteUnitCommand(id), cancellationToken)).ToActionResult();
}
