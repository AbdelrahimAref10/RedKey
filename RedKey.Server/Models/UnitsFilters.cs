using RedKey.Application.Features.Units.Common;
using RedKey.Application.Features.Units.GetUnits;

namespace RedKey.Server.Models;

/// <summary>Website filters (landing + customer). Bound from the query string.</summary>
public class PublicUnitsFilter
{
    /// <summary>Sale / rent / investment value from the lookups endpoint. Empty = all.</summary>
    public int? Purpose { get; init; }
    public int? UnitTypeId { get; init; }
    public int? CityId { get; init; }
    public int? AreaId { get; init; }
    public decimal? MaxPrice { get; init; }
    public string? Search { get; init; }
    public bool? Featured { get; init; }
    public int? Page { get; init; }
    /// <summary>Default 10.</summary>
    public int? PageSize { get; init; }

    /// <summary>Response language (ar / en), sent by the landing and customer clients (web + mobile).</summary>
    public string? Lang { get; init; }

    public GetUnitsQuery ToQuery(UnitsScope scope, int? status = null) => new()
    {
        Scope = scope,
        Purpose = Purpose,
        UnitTypeId = UnitTypeId,
        CityId = CityId,
        AreaId = AreaId,
        MaxPrice = MaxPrice,
        Search = Search,
        Featured = Featured,
        Status = status,
        Page = Page,
        PageSize = PageSize
    };
}

/// <summary>Dashboard filters (admin / broker): adds the review status.</summary>
public sealed class ManageUnitsFilter : PublicUnitsFilter
{
    /// <summary>Unit status value from the lookups endpoint. Empty = all.</summary>
    public int? Status { get; init; }

    public GetUnitsQuery ToQuery(UnitsScope scope) => ToQuery(scope, Status);
}
