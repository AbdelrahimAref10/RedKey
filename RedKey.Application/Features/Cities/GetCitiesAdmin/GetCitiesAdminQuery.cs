using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Cities.GetCitiesAdmin;

/// <summary>All cities (active + inactive) with both languages and their areas count, paged — super admin page.</summary>
public sealed record GetCitiesAdminQuery(string? Search, int? Page, int? PageSize) : IRequest<Result<PagedResult<CityAdminDto>>>;
