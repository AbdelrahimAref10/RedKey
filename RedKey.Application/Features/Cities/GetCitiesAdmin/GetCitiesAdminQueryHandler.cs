using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Cities.GetCitiesAdmin;

public sealed class GetCitiesAdminQueryHandler : IRequestHandler<GetCitiesAdminQuery, Result<PagedResult<CityAdminDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCitiesAdminQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<CityAdminDto>>> Handle(GetCitiesAdminQuery request, CancellationToken cancellationToken)
    {
        var paging = Paging.From(request.Page, request.PageSize);
        var query = _context.Cities.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(c => c.NameAr.Contains(s) || c.NameEn.Contains(s));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Id)
            .Skip(paging.Skip).Take(paging.PageSize)
            .Select(c => new CityAdminDto
            {
                Id = c.Id,
                NameAr = c.NameAr,
                NameEn = c.NameEn,
                IsActive = c.IsActive,
                SortOrder = c.SortOrder,
                AreasCount = _context.Areas.Count(a => a.CityId == c.Id)
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<CityAdminDto>>.Success(new PagedResult<CityAdminDto>
        {
            Items = items,
            Page = paging.Page,
            PageSize = paging.PageSize,
            TotalCount = total
        });
    }
}
