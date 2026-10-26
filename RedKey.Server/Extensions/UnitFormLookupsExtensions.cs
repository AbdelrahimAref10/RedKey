using MediatR;
using RedKey.Application.Features.Lookups.GetAreas;
using RedKey.Application.Features.Lookups.GetCities;
using RedKey.Application.Features.Lookups.GetLookups;
using RedKey.Application.Features.Lookups.GetUnitTypes;
using RedKey.Server.Models;

namespace RedKey.Server.Extensions;

public static class UnitFormLookupsExtensions
{
    /// <summary>Enums + active unit types + active cities and areas for the unit form (sequential: one DbContext).</summary>
    public static async Task<UnitFormLookupsDto> GetUnitFormLookupsAsync(this ISender sender, CancellationToken cancellationToken)
    {
        var enums = await sender.Send(new GetLookupsQuery(), cancellationToken);
        var types = await sender.Send(new GetUnitTypesQuery(), cancellationToken);
        var cities = await sender.Send(new GetCitiesQuery(), cancellationToken);
        var areas = await sender.Send(new GetAreasQuery(), cancellationToken);

        return new UnitFormLookupsDto
        {
            Enums = enums.Value!,
            UnitTypes = types.Value!,
            Cities = cities.Value!,
            Areas = areas.Value!
        };
    }
}
