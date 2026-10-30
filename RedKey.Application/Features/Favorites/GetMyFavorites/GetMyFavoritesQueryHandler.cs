using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Units;
using RedKey.Application.Features.Units.Common;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Favorites.GetMyFavorites;

public sealed class GetMyFavoritesQueryHandler : IRequestHandler<GetMyFavoritesQuery, Result<PagedResult<UnitListItemDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILanguageContext _language;

    public GetMyFavoritesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, ILanguageContext language)
    {
        _context = context;
        _currentUser = currentUser;
        _language = language;
    }

    public async Task<Result<PagedResult<UnitListItemDto>>> Handle(GetMyFavoritesQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result<PagedResult<UnitListItemDto>>.Failure(ErrorCodes.Units.Forbidden, 403);

        var paging = Paging.From(request.Page, request.PageSize);
        var ar = _language.IsArabic;

        // Only units the website still shows (a saved unit that was unpublished simply drops out).
        var favorites = _context.Favorites.AsNoTracking()
            .Where(f => f.CustomerUserId == userId && f.Unit.Status == UnitStatus.Approved);

        var total = await favorites.CountAsync(cancellationToken);
        var rows = await favorites
            .OrderByDescending(f => f.CreatedAtUtc)
            .Skip(paging.Skip).Take(paging.PageSize)
            .Select(f => f.Unit)
            .ToRows(ar)
            .ToListAsync(cancellationToken);

        return Result<PagedResult<UnitListItemDto>>.Success(new PagedResult<UnitListItemDto>
        {
            Items = rows.Select(r => r.ToListItem(UnitsScope.Public, ar)).ToList(),
            Page = paging.Page,
            PageSize = paging.PageSize,
            TotalCount = total
        });
    }
}
