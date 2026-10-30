using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.Favorites.AddFavorite;

/// <summary>Saves a published unit for the signed-in customer. Saving it again is a no-op.</summary>
public sealed record AddFavoriteCommand(Guid UnitId) : IRequest<Result>;
