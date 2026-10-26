using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Units;
using RedKey.Application.Features.Units.Common;

namespace RedKey.Application.Features.Units.GetUnits;

/// <summary>
/// The one query that lists units everywhere (landing, customer web/mobile, broker, admin).
/// <see cref="Scope"/> is set by the controller; everything else is a filter from the client.
/// </summary>
public sealed record GetUnitsQuery : IRequest<Result<PagedResult<UnitListItemDto>>>
{
    public required UnitsScope Scope { get; init; }

    /// <summary>Sale / rent / investment (see the lookups endpoint). Null = all.</summary>
    public int? Purpose { get; init; }
    public int? UnitTypeId { get; init; }
    public int? CityId { get; init; }
    public int? AreaId { get; init; }
    public decimal? MaxPrice { get; init; }
    public string? Search { get; init; }
    public bool? Featured { get; init; }

    /// <summary>Broker (own units) / admin only.</summary>
    public int? Status { get; init; }

    public int? Page { get; init; }
    public int? PageSize { get; init; }
}
