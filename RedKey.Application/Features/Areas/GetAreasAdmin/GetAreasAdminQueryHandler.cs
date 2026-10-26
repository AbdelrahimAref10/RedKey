using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Areas.GetAreasAdmin;

public sealed class GetAreasAdminQueryHandler : IRequestHandler<GetAreasAdminQuery, Result<PagedResult<AreaAdminDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAreasAdminQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<AreaAdminDto>>> Handle(GetAreasAdminQuery request, CancellationToken cancellationToken)
    {
        var paging = Paging.From(request.Page, request.PageSize);
        var query = _context.Areas.AsNoTracking().Where(a => a.CityId == request.CityId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(a => a.NameAr.Contains(s) || a.NameEn.Contains(s));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(a => a.SortOrder).ThenBy(a => a.Id)
            .Skip(paging.Skip).Take(paging.PageSize)
            .Select(a => new AreaAdminDto
            {
                Id = a.Id,
                CityId = a.CityId,
                NameAr = a.NameAr,
                NameEn = a.NameEn,
                IsActive = a.IsActive,
                SortOrder = a.SortOrder
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<AreaAdminDto>>.Success(new PagedResult<AreaAdminDto>
        {
            Items = items,
            Page = paging.Page,
            PageSize = paging.PageSize,
            TotalCount = total
        });
    }
}
