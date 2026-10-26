using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Units;
using RedKey.Application.Features.Units.Common;

namespace RedKey.Application.Features.Units.GetUnitById;

public sealed class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery, Result<UnitDetailsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILanguageContext _language;

    public GetUnitByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, ILanguageContext language)
    {
        _context = context;
        _currentUser = currentUser;
        _language = language;
    }

    public async Task<Result<UnitDetailsDto>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var ar = _language.IsArabic;
        var units = _context.Units.AsNoTracking()
            .ForScope(request.Scope, _currentUser.UserId)
            .Where(u => u.Id == request.Id);

        var row = await units.ToRows(ar).FirstOrDefaultAsync(cancellationToken);
        if (row is null)
            return Result<UnitDetailsDto>.NotFound(ErrorCodes.Units.NotFound);

        var extra = await units
            .Select(u => new
            {
                Description = ar ? u.Details.DescriptionAr : u.Details.DescriptionEn,
                Delivery = ar ? u.Details.DeliveryAr : u.Details.DeliveryEn,
                Images = u.Images.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList(),
                PaymentPlans = u.PaymentPlans.OrderBy(p => p.SortOrder)
                    .Select(p => new PaymentPlanDto { Label = ar ? p.LabelAr : p.LabelEn, Value = ar ? p.ValueAr : p.ValueEn })
                    .ToList(),
                Features = u.Features.OrderBy(f => f.SortOrder).Select(f => ar ? f.NameAr : f.NameEn).ToList()
            })
            .FirstAsync(cancellationToken);

        return Result<UnitDetailsDto>.Success(new UnitDetailsDto
        {
            Unit = row.ToListItem(request.Scope, ar),
            Description = extra.Description,
            Delivery = extra.Delivery,
            Images = extra.Images,
            PaymentPlans = extra.PaymentPlans,
            Features = extra.Features
        });
    }
}
