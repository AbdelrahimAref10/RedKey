using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Lookups.GetAreas;

public sealed class GetAreasQueryHandler : IRequestHandler<GetAreasQuery, Result<IReadOnlyList<AreaDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILanguageContext _language;

    public GetAreasQueryHandler(IApplicationDbContext context, ILanguageContext language)
    {
        _context = context;
        _language = language;
    }

    public async Task<Result<IReadOnlyList<AreaDto>>> Handle(GetAreasQuery request, CancellationToken cancellationToken)
    {
        var ar = _language.IsArabic;

        var areas = await _context.Areas.AsNoTracking()
            .Where(a => a.IsActive && a.City.IsActive)
            .Where(a => request.CityId == null || a.CityId == request.CityId)
            .OrderBy(a => a.SortOrder).ThenBy(a => a.Id)
            .Select(a => new AreaDto { Id = a.Id, CityId = a.CityId, Name = ar ? a.NameAr : a.NameEn })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<AreaDto>>.Success(areas);
    }
}
