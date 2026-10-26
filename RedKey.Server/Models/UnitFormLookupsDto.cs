using RedKey.Application.Dtos.Lookups;

namespace RedKey.Server.Models;

/// <summary>Everything the unit form needs in one call (dashboards).</summary>
public sealed class UnitFormLookupsDto
{
    public required LookupsDto Enums { get; init; }
    public required IReadOnlyList<UnitTypeDto> UnitTypes { get; init; }
    public required IReadOnlyList<CityDto> Cities { get; init; }
    /// <summary>All active areas; the form filters them by the selected city (<see cref="AreaDto.CityId"/>).</summary>
    public required IReadOnlyList<AreaDto> Areas { get; init; }
}
