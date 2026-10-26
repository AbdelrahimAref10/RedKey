using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Units;
using RedKey.Application.Features.Units.Common;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Units.GetUnits;

public sealed class GetUnitsQueryHandler : IRequestHandler<GetUnitsQuery, Result<PagedResult<UnitListItemDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILanguageContext _language;

    public GetUnitsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, ILanguageContext language)
    {
        _context = context;
        _currentUser = currentUser;
        _language = language;
    }

    public async Task<Result<PagedResult<UnitListItemDto>>> Handle(GetUnitsQuery request, CancellationToken cancellationToken)
    {
        var paging = Paging.From(request.Page, request.PageSize);
        var ar = _language.IsArabic;

        var query = _context.Units.AsNoTracking().ForScope(request.Scope, _currentUser.UserId);

        if (request.Purpose is { } purpose)
            query = query.Where(u => u.Purpose == (UnitPurpose)purpose);
        if (request.UnitTypeId is { } typeId)
            query = query.Where(u => u.UnitTypeId == typeId);
        if (request.CityId is { } cityId)
            query = query.Where(u => u.CityId == cityId);
        if (request.AreaId is { } areaId)
            query = query.Where(u => u.AreaId == areaId);
        if (request.MaxPrice is { } maxPrice)
            query = query.Where(u => u.Price <= maxPrice);
        if (request.Featured == true)
            query = query.Where(u => u.IsFeatured);
        if (request.Status is { } status && request.Scope is UnitsScope.BrokerOwn or UnitsScope.Admin)
            query = query.Where(u => u.Status == (UnitStatus)status);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(u =>
                u.Code.Contains(s) ||
                u.TitleAr.Contains(s) || u.TitleEn.Contains(s) ||
                u.CompoundAr.Contains(s) || u.CompoundEn.Contains(s));
        }

        var total = await query.CountAsync(cancellationToken);

        var rows = total == 0
            ? []
            : await query
                .OrderByDescending(u => u.CreatedAtUtc)
                .Skip(paging.Skip)
                .Take(paging.PageSize)
                .ToRows(ar)
                .ToListAsync(cancellationToken);

        return Result<PagedResult<UnitListItemDto>>.Success(new PagedResult<UnitListItemDto>
        {
            Items = rows.Select(r => r.ToListItem(request.Scope, ar)).ToList(),
            Page = paging.Page,
            PageSize = paging.PageSize,
            TotalCount = total
        });
    }
}
