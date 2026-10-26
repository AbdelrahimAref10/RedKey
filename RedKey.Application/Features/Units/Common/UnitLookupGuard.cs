using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Contracts.Persistence;

namespace RedKey.Application.Features.Units.Common;

internal static class UnitLookupGuard
{
    /// <summary>
    /// Checks the unit type and the area (and its city) are active, and returns the area city,
    /// which is saved on the unit too.
    /// </summary>
    public static async Task<(string? Error, int CityId)> CheckAsync(
        IApplicationDbContext context,
        int unitTypeId,
        int areaId,
        CancellationToken cancellationToken)
    {
        if (!await context.UnitTypes.AnyAsync(t => t.Id == unitTypeId && t.IsActive, cancellationToken))
            return (ErrorCodes.Units.InvalidUnitType, 0);

        var cityId = await context.Areas
            .Where(a => a.Id == areaId && a.IsActive && a.City.IsActive)
            .Select(a => (int?)a.CityId)
            .FirstOrDefaultAsync(cancellationToken);

        return cityId is null ? (ErrorCodes.Units.InvalidArea, 0) : (null, cityId.Value);
    }
}
