using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Lookups.GetUnitTypes;

public sealed class GetUnitTypesQueryHandler : IRequestHandler<GetUnitTypesQuery, Result<IReadOnlyList<UnitTypeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILanguageContext _language;

    public GetUnitTypesQueryHandler(IApplicationDbContext context, ILanguageContext language)
    {
        _context = context;
        _language = language;
    }

    public async Task<Result<IReadOnlyList<UnitTypeDto>>> Handle(GetUnitTypesQuery request, CancellationToken cancellationToken)
    {
        var ar = _language.IsArabic;

        var types = await _context.UnitTypes.AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.SortOrder).ThenBy(t => t.Id)
            .Select(t => new UnitTypeDto { Id = t.Id, Name = ar ? t.NameAr : t.NameEn, IconUrl = t.IconUrl })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<UnitTypeDto>>.Success(types);
    }
}
