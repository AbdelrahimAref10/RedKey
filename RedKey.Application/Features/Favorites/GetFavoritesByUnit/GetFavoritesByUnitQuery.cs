using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Favorites;

namespace RedKey.Application.Features.Favorites.GetFavoritesByUnit;

/// <summary>Units ranked by how many customers saved them (+ how many of those asked for a viewing), super admin.</summary>
public sealed record GetFavoritesByUnitQuery(int? Page, int? PageSize) : IRequest<Result<PagedResult<FavoriteUnitStatsDto>>>;
