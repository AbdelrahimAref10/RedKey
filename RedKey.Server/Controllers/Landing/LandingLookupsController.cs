using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Dtos.Lookups;
using RedKey.Application.Features.Lookups.GetAreas;
using RedKey.Application.Features.Lookups.GetCities;
using RedKey.Application.Features.Lookups.GetLookups;
using RedKey.Application.Features.Lookups.GetUnitTypes;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.Landing;

/// <summary>Dropdown / section data of the public website, translated with <c>?lang=</c>.</summary>
[Route("api/landing")]
public sealed class LandingLookupsController : LandingControllerBase
{
    private readonly ISender _sender;

    public LandingLookupsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Active unit types with their icons (landing "browse by type" section + filters).</summary>
    [HttpGet("unit-types")]
    [ProducesResponseType<IReadOnlyList<UnitTypeDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnitTypes([FromQuery] string? lang, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetUnitTypesQuery(), cancellationToken)).ToActionResult();

    /// <summary>Active cities.</summary>
    [HttpGet("cities")]
    [ProducesResponseType<IReadOnlyList<CityDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCities([FromQuery] string? lang, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetCitiesQuery(), cancellationToken)).ToActionResult();

    /// <summary>Active areas; <c>cityId</c> limits them to one city.</summary>
    [HttpGet("areas")]
    [ProducesResponseType<IReadOnlyList<AreaDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAreas([FromQuery] int? cityId, [FromQuery] string? lang, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetAreasQuery(cityId), cancellationToken)).ToActionResult();

    /// <summary>Translated enums (purposes, finishing, preferred times...).</summary>
    [HttpGet("lookups")]
    [ProducesResponseType<LookupsDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLookups([FromQuery] string? lang, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetLookupsQuery(), cancellationToken)).ToActionResult();
}
