using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Favorites;
using RedKey.Application.Features.Favorites.GetFavoritesAdmin;
using RedKey.Application.Features.Favorites.GetFavoritesByUnit;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.SuperAdmin;

/// <summary>What customers saved — for follow-up calls (who is interested in what).</summary>
[Route("api/admin/favorites")]
public sealed class AdminFavoritesController : SuperAdminControllerBase
{
    private readonly ISender _sender;

    public AdminFavoritesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Every favorite with its customer (name, phone, email) and unit data, newest first.
    /// <c>search</c> = customer name / phone / email or unit code; <c>unitId</c> = the savers of one unit.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<PagedResult<FavoriteAdminDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavorites(
        [FromQuery] string? search,
        [FromQuery] Guid? unitId,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken) =>
        (await _sender.Send(new GetFavoritesAdminQuery(search, unitId, page, pageSize), cancellationToken)).ToActionResult();

    /// <summary>Units ranked by how many customers saved them.</summary>
    [HttpGet("by-unit")]
    [ProducesResponseType<PagedResult<FavoriteUnitStatsDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUnit([FromQuery] int? page, [FromQuery] int? pageSize, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetFavoritesByUnitQuery(page, pageSize), cancellationToken)).ToActionResult();
}
