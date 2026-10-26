using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.UnitTypes.GetUnitTypesAdmin;

/// <summary>All unit types (active + inactive) with both languages, paged — super admin page.</summary>
public sealed record GetUnitTypesAdminQuery(string? Search, int? Page, int? PageSize) : IRequest<Result<PagedResult<UnitTypeAdminDto>>>;
