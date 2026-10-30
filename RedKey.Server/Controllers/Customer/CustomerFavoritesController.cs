using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Units;
using RedKey.Application.Features.Favorites.AddFavorite;
using RedKey.Application.Features.Favorites.GetMyFavoriteIds;
using RedKey.Application.Features.Favorites.GetMyFavorites;
using RedKey.Application.Features.Favorites.RemoveFavorite;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.Customer;

/// <summary>The signed-in customer's saved units — the same endpoints for the website and the mobile app.</summary>
[Route("api/customer/favorites")]
public sealed class CustomerFavoritesController : CustomerControllerBase
{
    private readonly ISender _sender;

    public CustomerFavoritesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Saved units (card shape, newest saved first), paged by 10. Translated with <c>?lang=</c>.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResult<UnitListItemDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavorites([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? lang, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetMyFavoritesQuery(page, pageSize), cancellationToken)).ToActionResult();

    /// <summary>Only the saved unit ids (to light the hearts on the cards).</summary>
    [HttpGet("ids")]
    [ProducesResponseType<IReadOnlyList<Guid>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavoriteIds(CancellationToken cancellationToken) =>
        (await _sender.Send(new GetMyFavoriteIdsQuery(), cancellationToken)).ToActionResult();

    /// <summary>Saves a published unit (idempotent: saving it twice is fine).</summary>
    [HttpPut("{unitId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Add(Guid unitId, CancellationToken cancellationToken) =>
        (await _sender.Send(new AddFavoriteCommand(unitId), cancellationToken)).ToActionResult();

    /// <summary>Removes it (no error when it was not saved).</summary>
    [HttpDelete("{unitId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remove(Guid unitId, CancellationToken cancellationToken) =>
        (await _sender.Send(new RemoveFavoriteCommand(unitId), cancellationToken)).ToActionResult();
}
