using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.Favorites.RemoveFavorite;

/// <summary>Removes a unit from the signed-in customer's favorites (no-op when it is not there).</summary>
public sealed record RemoveFavoriteCommand(Guid UnitId) : IRequest<Result>;
