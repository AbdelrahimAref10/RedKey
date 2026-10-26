using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Areas.GetAreasAdmin;

/// <summary>The areas of one city (active + inactive) with both languages, paged — super admin page.</summary>
public sealed record GetAreasAdminQuery(int CityId, string? Search, int? Page, int? PageSize) : IRequest<Result<PagedResult<AreaAdminDto>>>;
