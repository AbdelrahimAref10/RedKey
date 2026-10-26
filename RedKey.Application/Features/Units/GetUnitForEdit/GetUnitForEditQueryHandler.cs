using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Localization;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Units;
using RedKey.Application.Features.Units.Common;

namespace RedKey.Application.Features.Units.GetUnitForEdit;

public sealed class GetUnitForEditQueryHandler : IRequestHandler<GetUnitForEditQuery, Result<UnitEditDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILanguageContext _language;

    public GetUnitForEditQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, ILanguageContext language)
    {
        _context = context;
        _currentUser = currentUser;
        _language = language;
    }

    public async Task<Result<UnitEditDto>> Handle(GetUnitForEditQuery request, CancellationToken cancellationToken)
    {
        var ar = _language.IsArabic;

        var unit = await _context.Units.AsNoTracking()
            .ForScope(request.Scope, _currentUser.UserId)
            .Where(u => u.Id == request.Id)
            .Include(u => u.Details)
            .Include(u => u.Images)
            .Include(u => u.PaymentPlans)
            .Include(u => u.Features)
            .Include(u => u.CreatedBy)
            .FirstOrDefaultAsync(cancellationToken);

        if (unit is null)
            return Result<UnitEditDto>.NotFound(ErrorCodes.Units.NotFound);

        return Result<UnitEditDto>.Success(new UnitEditDto
        {
            Id = unit.Id,
            Code = unit.Code,
            UnitTypeId = unit.UnitTypeId,
            CityId = unit.CityId,
            AreaId = unit.AreaId,
            Purpose = (int)unit.Purpose,
            TitleAr = unit.TitleAr,
            TitleEn = unit.TitleEn,
            CompoundAr = unit.CompoundAr,
            CompoundEn = unit.CompoundEn,
            Price = unit.Price,
            Bedrooms = unit.Bedrooms,
            Bathrooms = unit.Bathrooms,
            Size = unit.Size,
            Finishing = (int)unit.Finishing,
            IsFeatured = unit.IsFeatured,
            BrokerCommission = unit.BrokerCommission,
            ViewingFee = unit.ViewingFee,
            DescriptionAr = unit.Details.DescriptionAr,
            DescriptionEn = unit.Details.DescriptionEn,
            DeliveryAr = unit.Details.DeliveryAr,
            DeliveryEn = unit.Details.DeliveryEn,
            CoverImageUrl = unit.CoverImageUrl,
            GalleryImages = unit.Images.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList(),
            PaymentPlans = unit.PaymentPlans.OrderBy(p => p.SortOrder)
                .Select(p => new PaymentPlanInput { LabelAr = p.LabelAr, LabelEn = p.LabelEn, ValueAr = p.ValueAr, ValueEn = p.ValueEn })
                .ToList(),
            Features = unit.Features.OrderBy(f => f.SortOrder)
                .Select(f => new FeatureInput { NameAr = f.NameAr, NameEn = f.NameEn })
                .ToList(),
            Status = (int)unit.Status,
            StatusName = EnumNames.Of(unit.Status, ar),
            ReviewNote = unit.ReviewNote,
            CreatedBy = request.Scope == UnitsScope.Admin
                ? new UnitCreatorDto
                {
                    UserId = unit.CreatedByUserId,
                    FullName = unit.CreatedBy.FullName,
                    Role = (int)unit.CreatedByRole,
                    RoleName = EnumNames.Of(unit.CreatedByRole, ar)
                }
                : null
        });
    }
}
