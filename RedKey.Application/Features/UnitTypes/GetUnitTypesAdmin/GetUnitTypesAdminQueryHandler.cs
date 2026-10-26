using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.UnitTypes.GetUnitTypesAdmin;

public sealed class GetUnitTypesAdminQueryHandler : IRequestHandler<GetUnitTypesAdminQuery, Result<PagedResult<UnitTypeAdminDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetUnitTypesAdminQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<UnitTypeAdminDto>>> Handle(GetUnitTypesAdminQuery request, CancellationToken cancellationToken)
    {
        var paging = Paging.From(request.Page, request.PageSize);
        var query = _context.UnitTypes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(t => t.NameAr.Contains(s) || t.NameEn.Contains(s));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(t => t.SortOrder).ThenBy(t => t.Id)
            .Skip(paging.Skip).Take(paging.PageSize)
            .Select(t => new UnitTypeAdminDto
            {
                Id = t.Id,
                NameAr = t.NameAr,
                NameEn = t.NameEn,
                IconUrl = t.IconUrl,
                IsActive = t.IsActive,
                SortOrder = t.SortOrder
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<UnitTypeAdminDto>>.Success(new PagedResult<UnitTypeAdminDto>
        {
            Items = items,
            Page = paging.Page,
            PageSize = paging.PageSize,
            TotalCount = total
        });
    }
}
