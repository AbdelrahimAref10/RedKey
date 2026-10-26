using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Localization;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.ViewingRequests;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.ViewingRequests.GetViewingRequestsAdmin;

public sealed class GetViewingRequestsAdminQueryHandler
    : IRequestHandler<GetViewingRequestsAdminQuery, Result<PagedResult<ViewingRequestAdminDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILanguageContext _language;

    public GetViewingRequestsAdminQueryHandler(IApplicationDbContext context, ILanguageContext language)
    {
        _context = context;
        _language = language;
    }

    public async Task<Result<PagedResult<ViewingRequestAdminDto>>> Handle(
        GetViewingRequestsAdminQuery request,
        CancellationToken cancellationToken)
    {
        var paging = Paging.From(request.Page, request.PageSize);
        var ar = _language.IsArabic;
        var query = _context.ViewingRequests.AsNoTracking();

        if (request.Status is { } status)
            query = query.Where(v => v.Status == (ViewingRequestStatus)status);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(v => v.Name.Contains(s) || v.PhoneNumber.Contains(s) || v.Unit.Code.Contains(s));
        }

        var total = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(v => v.CreatedAtUtc)
            .Skip(paging.Skip).Take(paging.PageSize)
            .Select(v => new
            {
                v.Id,
                v.Name,
                v.PhoneNumber,
                v.PreferredTime,
                v.Status,
                v.UnitId,
                v.Unit.Code,
                Title = ar ? v.Unit.TitleAr : v.Unit.TitleEn,
                Area = ar ? v.Unit.Area.NameAr : v.Unit.Area.NameEn,
                City = ar ? v.Unit.City.NameAr : v.Unit.City.NameEn,
                v.ViewingFee,
                v.CompletedAtUtc,
                v.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        // Enum names are added in memory (single source: EnumNames).
        var items = rows.Select(r => new ViewingRequestAdminDto
        {
            Id = r.Id,
            Name = r.Name,
            PhoneNumber = r.PhoneNumber,
            PreferredTime = (int)r.PreferredTime,
            PreferredTimeName = EnumNames.Of(r.PreferredTime, ar),
            Status = (int)r.Status,
            StatusName = EnumNames.Of(r.Status, ar),
            UnitId = r.UnitId,
            UnitCode = r.Code,
            UnitTitle = r.Title,
            AreaName = r.Area,
            CityName = r.City,
            ViewingFee = r.ViewingFee,
            CompletedAtUtc = r.CompletedAtUtc,
            CreatedAtUtc = r.CreatedAtUtc
        }).ToList();

        return Result<PagedResult<ViewingRequestAdminDto>>.Success(new PagedResult<ViewingRequestAdminDto>
        {
            Items = items,
            Page = paging.Page,
            PageSize = paging.PageSize,
            TotalCount = total
        });
    }
}
