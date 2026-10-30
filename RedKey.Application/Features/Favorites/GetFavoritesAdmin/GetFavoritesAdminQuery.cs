using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Favorites;

namespace RedKey.Application.Features.Favorites.GetFavoritesAdmin;

/// <summary>
/// Every saved unit with its customer (name, phone, email) and the unit data, newest first (super admin).
/// <paramref name="Search"/> = customer name / phone / email or unit code; <paramref name="UnitId"/> = one unit's savers.
/// </summary>
public sealed record GetFavoritesAdminQuery(string? Search, Guid? UnitId, int? Page, int? PageSize)
    : IRequest<Result<PagedResult<FavoriteAdminDto>>>;
