using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Favorites;
using RedKey.Application.Features.Units.Common;

namespace RedKey.Application.Features.Favorites.GetFavoritesByUnit;

public sealed class GetFavoritesByUnitQueryHandler : IRequestHandler<GetFavoritesByUnitQuery, Result<PagedResult<FavoriteUnitStatsDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILanguageContext _language;

    public GetFavoritesByUnitQueryHandler(IApplicationDbContext context, ILanguageContext language)
    {
        _context = context;
        _language = language;
    }

    public async Task<Result<PagedResult<FavoriteUnitStatsDto>>> Handle(GetFavoritesByUnitQuery request, CancellationToken cancellationToken)
    {
        var paging = Paging.From(request.Page, request.PageSize);
        var ar = _language.IsArabic;

        var grouped = _context.Favorites.AsNoTracking()
            .GroupBy(f => f.UnitId)
            .Select(g => new
            {
                UnitId = g.Key,
                Saved = g.Count(),
                // Savers of this unit who also asked to view it.
                Requested = g.Count(f => _context.ViewingRequests.Any(v => v.UnitId == f.UnitId && v.CustomerUserId == f.CustomerUserId)),
                Last = g.Max(f => f.CreatedAtUtc)
            });

        var total = await grouped.CountAsync(cancellationToken);
        var stats = await grouped
            .OrderByDescending(x => x.Saved).ThenByDescending(x => x.Last)
            .Skip(paging.Skip).Take(paging.PageSize)
            .ToListAsync(cancellationToken);

        var unitIds = stats.Select(s => s.UnitId).ToList();
        var units = (await _context.Units.AsNoTracking()
                .Where(u => unitIds.Contains(u.Id))
                .ToRows(ar)
                .ToListAsync(cancellationToken))
            .ToDictionary(u => u.Id, u => u.ToListItem(UnitsScope.Admin, ar));

        var items = stats
            .Where(s => units.ContainsKey(s.UnitId))
            .Select(s => new FavoriteUnitStatsDto
            {
                Unit = units[s.UnitId],
                SavedCount = s.Saved,
                RequestedViewingCount = s.Requested,
                LastSavedAtUtc = s.Last
            })
            .ToList();

        return Result<PagedResult<FavoriteUnitStatsDto>>.Success(new PagedResult<FavoriteUnitStatsDto>
        {
            Items = items,
            Page = paging.Page,
            PageSize = paging.PageSize,
            TotalCount = total
        });
    }
}
