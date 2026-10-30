using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Localization;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Favorites;
using RedKey.Application.Features.Units.Common;

namespace RedKey.Application.Features.Favorites.GetFavoritesAdmin;

public sealed class GetFavoritesAdminQueryHandler : IRequestHandler<GetFavoritesAdminQuery, Result<PagedResult<FavoriteAdminDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILanguageContext _language;

    public GetFavoritesAdminQueryHandler(IApplicationDbContext context, ILanguageContext language)
    {
        _context = context;
        _language = language;
    }

    public async Task<Result<PagedResult<FavoriteAdminDto>>> Handle(GetFavoritesAdminQuery request, CancellationToken cancellationToken)
    {
        var paging = Paging.From(request.Page, request.PageSize);
        var ar = _language.IsArabic;

        var favorites = _context.Favorites.AsNoTracking();
        if (request.UnitId is { } unitId)
            favorites = favorites.Where(f => f.UnitId == unitId);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            favorites = favorites.Where(f =>
                f.Customer.FullName.Contains(s) ||
                (f.Customer.PhoneNumber != null && f.Customer.PhoneNumber.Contains(s)) ||
                (f.Customer.Email != null && f.Customer.Email.Contains(s)) ||
                f.Unit.Code.Contains(s));
        }

        var total = await favorites.CountAsync(cancellationToken);
        var rows = await favorites
            .OrderByDescending(f => f.CreatedAtUtc)
            .Skip(paging.Skip).Take(paging.PageSize)
            .Select(f => new
            {
                f.CustomerUserId,
                f.Customer.FullName,
                f.Customer.PhoneNumber,
                f.Customer.Email,
                f.UnitId,
                f.CreatedAtUtc,
                // Did the same customer ask to view it? (latest request)
                ViewingStatus = _context.ViewingRequests
                    .Where(v => v.UnitId == f.UnitId && v.CustomerUserId == f.CustomerUserId)
                    .OrderByDescending(v => v.CreatedAtUtc)
                    .Select(v => (Domain.Enums.ViewingRequestStatus?)v.Status)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        // The units of this page in one query, in the admin card shape.
        var unitIds = rows.Select(r => r.UnitId).Distinct().ToList();
        var units = (await _context.Units.AsNoTracking()
                .Where(u => unitIds.Contains(u.Id))
                .ToRows(ar)
                .ToListAsync(cancellationToken))
            .ToDictionary(u => u.Id, u => u.ToListItem(UnitsScope.Admin, ar));

        var items = rows
            .Where(r => units.ContainsKey(r.UnitId))
            .Select(r => new FavoriteAdminDto
            {
                CustomerUserId = r.CustomerUserId,
                CustomerName = r.FullName,
                CustomerPhone = r.PhoneNumber,
                CustomerEmail = r.Email,
                Unit = units[r.UnitId],
                SavedAtUtc = r.CreatedAtUtc,
                RequestedViewing = r.ViewingStatus is not null,
                ViewingStatusName = r.ViewingStatus is { } status ? EnumNames.Of(status, ar) : null
            })
            .ToList();

        return Result<PagedResult<FavoriteAdminDto>>.Success(new PagedResult<FavoriteAdminDto>
        {
            Items = items,
            Page = paging.Page,
            PageSize = paging.PageSize,
            TotalCount = total
        });
    }
}
