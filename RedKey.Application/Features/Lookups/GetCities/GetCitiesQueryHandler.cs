using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Lookups.GetCities;

public sealed class GetCitiesQueryHandler : IRequestHandler<GetCitiesQuery, Result<IReadOnlyList<CityDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILanguageContext _language;

    public GetCitiesQueryHandler(IApplicationDbContext context, ILanguageContext language)
    {
        _context = context;
        _language = language;
    }

    public async Task<Result<IReadOnlyList<CityDto>>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        var ar = _language.IsArabic;

        var cities = await _context.Cities.AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Id)
            .Select(c => new CityDto { Id = c.Id, Name = ar ? c.NameAr : c.NameEn })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<CityDto>>.Success(cities);
    }
}
