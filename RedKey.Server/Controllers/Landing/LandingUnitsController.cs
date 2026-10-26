using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Units;
using RedKey.Application.Features.Units.Common;
using RedKey.Application.Features.Units.GetUnitById;
using RedKey.Server.Extensions;
using RedKey.Server.Models;

namespace RedKey.Server.Controllers.Landing;

/// <summary>Published units for visitors (same query as every other units endpoint, public scope).</summary>
[Route("api/landing/units")]
public sealed class LandingUnitsController : LandingControllerBase
{
    private readonly ISender _sender;

    public LandingUnitsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Published units, 10 per page, filtered by purpose / type / area / price / search.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResult<UnitListItemDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnits([FromQuery] PublicUnitsFilter filter, CancellationToken cancellationToken) =>
        (await _sender.Send(filter.ToQuery(UnitsScope.Public), cancellationToken)).ToActionResult();

    [HttpGet("{id:guid}")]
    [ProducesResponseType<UnitDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUnit(Guid id, [FromQuery] string? lang, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetUnitByIdQuery(id, UnitsScope.Public), cancellationToken)).ToActionResult();
}
