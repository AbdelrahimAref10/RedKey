using FluentValidation;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Localization;
using RedKey.Application.Features.Units.Common;
using RedKey.Application.Features.Units.CreateUnit;
using RedKey.Application.Features.Units.ReviewUnit;
using RedKey.Application.Features.Units.UpdateUnit;
using RedKey.Domain.Enums;

namespace RedKey.Application.Validators.Units;

/// <summary>Rules shared by create + update (both use <see cref="UnitInput"/>).</summary>
public abstract class UnitInputValidator<T> : AbstractValidator<T> where T : UnitInput
{
    public const int MaxGalleryImages = 3;
    private const int MaxListItems = 20;

    protected UnitInputValidator()
    {
        RuleFor(x => x.UnitTypeId).GreaterThan(0).WithMessage(ErrorCodes.Validation.Required);
        RuleFor(x => x.AreaId).GreaterThan(0).WithMessage(ErrorCodes.Validation.Required);
        RuleFor(x => x.Purpose).Must(EnumNames.IsDefined<UnitPurpose>).WithMessage(ErrorCodes.Validation.InvalidValue);
        RuleFor(x => x.Finishing).Must(EnumNames.IsDefined<FinishingType>).WithMessage(ErrorCodes.Validation.InvalidValue);

        Text(x => x.TitleAr, 200);
        Text(x => x.TitleEn, 200);
        Text(x => x.CompoundAr, 150);
        Text(x => x.CompoundEn, 150);
        Text(x => x.DescriptionAr, 4000);
        Text(x => x.DescriptionEn, 4000);
        RuleFor(x => x.DeliveryAr).MaximumLength(100).WithMessage(ErrorCodes.Validation.MaxLength);
        RuleFor(x => x.DeliveryEn).MaximumLength(100).WithMessage(ErrorCodes.Validation.MaxLength);

        RuleFor(x => x.Price).GreaterThan(0).WithMessage(ErrorCodes.Validation.InvalidValue);
        RuleFor(x => x.Size).GreaterThan(0).WithMessage(ErrorCodes.Validation.InvalidValue);
        RuleFor(x => x.Bedrooms).InclusiveBetween(0, 50).WithMessage(ErrorCodes.Validation.InvalidValue);
        RuleFor(x => x.Bathrooms).InclusiveBetween(0, 50).WithMessage(ErrorCodes.Validation.InvalidValue);
        RuleFor(x => x.BrokerCommission).InclusiveBetween(0, 100).WithMessage(ErrorCodes.Validation.InvalidValue);
        RuleFor(x => x.ViewingFee).InclusiveBetween(0, 1_000_000).When(x => x.ViewingFee.HasValue).WithMessage(ErrorCodes.Validation.InvalidValue);

        RuleFor(x => x.CoverImage).NotEmpty().WithMessage(ErrorCodes.Units.CoverRequired);
        RuleFor(x => x.GalleryImages)
            .Must(images => images.Count <= MaxGalleryImages).WithMessage(ErrorCodes.Units.TooManyImages);
        RuleForEach(x => x.GalleryImages).NotEmpty().WithMessage(ErrorCodes.Units.InvalidImage);

        RuleFor(x => x.PaymentPlans).Must(p => p.Count <= MaxListItems).WithMessage(ErrorCodes.Validation.InvalidValue);
        RuleForEach(x => x.PaymentPlans).ChildRules(plan =>
        {
            plan.RuleFor(p => p.LabelAr).NotEmpty().MaximumLength(150).WithMessage(ErrorCodes.Validation.Required);
            plan.RuleFor(p => p.LabelEn).NotEmpty().MaximumLength(150).WithMessage(ErrorCodes.Validation.Required);
            plan.RuleFor(p => p.ValueAr).NotEmpty().MaximumLength(150).WithMessage(ErrorCodes.Validation.Required);
            plan.RuleFor(p => p.ValueEn).NotEmpty().MaximumLength(150).WithMessage(ErrorCodes.Validation.Required);
        });

        RuleFor(x => x.Features).Must(f => f.Count <= MaxListItems).WithMessage(ErrorCodes.Validation.InvalidValue);
        RuleForEach(x => x.Features).ChildRules(feature =>
        {
            feature.RuleFor(f => f.NameAr).NotEmpty().MaximumLength(100).WithMessage(ErrorCodes.Validation.Required);
            feature.RuleFor(f => f.NameEn).NotEmpty().MaximumLength(100).WithMessage(ErrorCodes.Validation.Required);
        });
    }

    private void Text(System.Linq.Expressions.Expression<Func<T, string>> field, int max) =>
        RuleFor(field)
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .MaximumLength(max).WithMessage(ErrorCodes.Validation.MaxLength);
}

public sealed class CreateUnitCommandValidator : UnitInputValidator<CreateUnitCommand>;

public sealed class UpdateUnitCommandValidator : UnitInputValidator<UpdateUnitCommand>;

public sealed class ReviewUnitCommandValidator : AbstractValidator<ReviewUnitCommand>
{
    public ReviewUnitCommandValidator()
    {
        RuleFor(x => x.Decision).Must(EnumNames.IsDefined<UnitReviewDecision>).WithMessage(ErrorCodes.Validation.InvalidValue);
        RuleFor(x => x.Note).MaximumLength(1000).WithMessage(ErrorCodes.Validation.MaxLength);
        RuleFor(x => x.Note)
            .NotEmpty().When(x => x.Decision != (int)UnitReviewDecision.Approve)
            .WithMessage(ErrorCodes.Validation.Required);
    }
}
