using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Units;
using RedKey.Application.Features.Units.Common;
using RedKey.Application.Features.Units.CreateUnit;
using RedKey.Application.Features.Units.DeleteUnit;
using RedKey.Application.Features.Units.GetUnitById;
using RedKey.Application.Features.Units.GetUnitForEdit;
using RedKey.Application.Features.Units.ReviewUnit;
using RedKey.Application.Features.Units.UpdateUnit;
using RedKey.Server.Extensions;
using RedKey.Server.Models;

namespace RedKey.Server.Controllers.SuperAdmin;

/// <summary>
/// Every unit with its creator. Create / update use the same commands as brokers
/// (a super admin unit is published right away). Review approves / rejects broker units.
/// </summary>
[Route("api/admin/units")]
public sealed class AdminUnitsController : SuperAdminControllerBase
{
    private const long MaxUnitRequestBytes = 32 * 1024 * 1024;
    private readonly ISender _sender;

    public AdminUnitsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType<PagedResult<UnitListItemDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnits([FromQuery] ManageUnitsFilter filter, CancellationToken cancellationToken) =>
        (await _sender.Send(filter.ToQuery(UnitsScope.Admin), cancellationToken)).ToActionResult();

    [HttpGet("{id:guid}")]
    [ProducesResponseType<UnitDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUnit(Guid id, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetUnitByIdQuery(id, UnitsScope.Admin), cancellationToken)).ToActionResult();

    [HttpGet("{id:guid}/edit")]
    [ProducesResponseType<UnitEditDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUnitForEdit(Guid id, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetUnitForEditQuery(id, UnitsScope.Admin), cancellationToken)).ToActionResult();

    /// <summary>Unit types, areas and enums for the unit form.</summary>
    [HttpGet("form-lookups")]
    [ProducesResponseType<UnitFormLookupsDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFormLookups(CancellationToken cancellationToken) =>
        Ok(await _sender.GetUnitFormLookupsAsync(cancellationToken));

    [HttpPost]
    [RequestSizeLimit(MaxUnitRequestBytes)]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUnitCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command, cancellationToken)).ToActionResult();

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

    /// <summary>Approve / request changes / reject (the broker is notified).</summary>
    [HttpPost("{id:guid}/review")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Review(Guid id, [FromBody] ReviewUnitCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command with { Id = id }, cancellationToken)).ToActionResult();
}
