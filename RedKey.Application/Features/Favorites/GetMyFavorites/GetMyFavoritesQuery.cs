using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Units;

namespace RedKey.Application.Features.Favorites.GetMyFavorites;

/// <summary>The signed-in customer's saved units (published ones), newest saved first — same card shape as the listings.</summary>
public sealed record GetMyFavoritesQuery(int? Page, int? PageSize) : IRequest<Result<PagedResult<UnitListItemDto>>>;
