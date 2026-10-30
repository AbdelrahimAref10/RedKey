using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.Favorites.GetMyFavoriteIds;

/// <summary>Ids of the customer's saved (published) units — lights the hearts on the cards without loading the units.</summary>
public sealed record GetMyFavoriteIdsQuery : IRequest<Result<IReadOnlyList<Guid>>>;
