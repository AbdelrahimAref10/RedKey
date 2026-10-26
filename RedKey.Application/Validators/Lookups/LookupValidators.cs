using FluentValidation;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Localization;
using RedKey.Application.Features.Account.UpdateMyLanguage;
using RedKey.Application.Features.Areas.CreateArea;
using RedKey.Application.Features.Areas.UpdateArea;
using RedKey.Application.Features.Cities.CreateCity;
using RedKey.Application.Features.ContactMessages.Common;
using RedKey.Application.Features.ContactMessages.CreateContactMessage;
using RedKey.Application.Features.Cities.UpdateCity;
using RedKey.Application.Features.UnitTypes.CreateUnitType;
using RedKey.Application.Features.UnitTypes.UpdateUnitType;
using RedKey.Application.Features.ViewingRequests.CreateViewingRequest;
using RedKey.Application.Features.ViewingRequests.UpdateViewingRequestStatus;
using RedKey.Domain.Enums;

namespace RedKey.Application.Validators.Lookups;

internal static class NameRules
{
    public static void Names<T>(AbstractValidator<T> v, Func<T, string> ar, Func<T, string> en)
    {
        v.RuleFor(x => ar(x))
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .MaximumLength(100).WithMessage(ErrorCodes.Validation.MaxLength)
            .OverridePropertyName("NameAr");
        v.RuleFor(x => en(x))
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .MaximumLength(100).WithMessage(ErrorCodes.Validation.MaxLength)
            .OverridePropertyName("NameEn");
    }
}

public sealed class CreateUnitTypeCommandValidator : AbstractValidator<CreateUnitTypeCommand>
{
    public CreateUnitTypeCommandValidator() => NameRules.Names(this, x => x.NameAr, x => x.NameEn);
}

public sealed class UpdateUnitTypeCommandValidator : AbstractValidator<UpdateUnitTypeCommand>
{
    public UpdateUnitTypeCommandValidator() => NameRules.Names(this, x => x.NameAr, x => x.NameEn);
}

public sealed class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator() => NameRules.Names(this, x => x.NameAr, x => x.NameEn);
}

public sealed class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
{
    public UpdateCityCommandValidator() => NameRules.Names(this, x => x.NameAr, x => x.NameEn);
}

public sealed class CreateAreaCommandValidator : AbstractValidator<CreateAreaCommand>
{
    public CreateAreaCommandValidator()
    {
        RuleFor(x => x.CityId).GreaterThan(0).WithMessage(ErrorCodes.Validation.Required);
        NameRules.Names(this, x => x.NameAr, x => x.NameEn);
    }
}

public sealed class UpdateAreaCommandValidator : AbstractValidator<UpdateAreaCommand>
{
    public UpdateAreaCommandValidator() => NameRules.Names(this, x => x.NameAr, x => x.NameEn);
}

public sealed class CreateViewingRequestCommandValidator : AbstractValidator<CreateViewingRequestCommand>
{
    public CreateViewingRequestCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .MaximumLength(150).WithMessage(ErrorCodes.Validation.MaxLength);
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .Matches(@"^\+?[0-9 ]{8,18}$").WithMessage(ErrorCodes.Validation.InvalidPhone);
        RuleFor(x => x.PreferredTime)
            .Must(EnumNames.IsDefined<PreferredTime>).WithMessage(ErrorCodes.Validation.InvalidValue);
    }
}

public sealed class UpdateViewingRequestStatusCommandValidator : AbstractValidator<UpdateViewingRequestStatusCommand>
{
    public UpdateViewingRequestStatusCommandValidator()
    {
        RuleFor(x => x.Status)
            .Must(EnumNames.IsDefined<ViewingRequestStatus>).WithMessage(ErrorCodes.Validation.InvalidValue);
    }
}

public sealed class CreateContactMessageCommandValidator : AbstractValidator<CreateContactMessageCommand>
{
    public CreateContactMessageCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .MaximumLength(150).WithMessage(ErrorCodes.Validation.MaxLength);
        RuleFor(x => x.CountryIso)
            .Matches("^[A-Za-z]{2}$").WithMessage(ErrorCodes.Validation.InvalidValue);
        RuleFor(x => x.DialCode)
            .Must(code => PhoneDigits.DialCode(code ?? string.Empty).Length is >= 1 and <= 4)
            .WithMessage(ErrorCodes.Validation.InvalidValue);
        // E.164: at most 15 digits with the country code; national part 6-14 after the leading 0 is dropped.
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .Must(phone => PhoneDigits.National(phone ?? string.Empty).Length is >= 6 and <= 14)
            .WithMessage(ErrorCodes.Validation.InvalidPhone);
        RuleFor(x => x.Subject)
            .Must(EnumNames.IsDefined<ContactSubject>).WithMessage(ErrorCodes.Validation.InvalidValue);
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .MaximumLength(2000).WithMessage(ErrorCodes.Validation.MaxLength);
    }
}

public sealed class UpdateMyLanguageCommandValidator : AbstractValidator<UpdateMyLanguageCommand>
{
    public UpdateMyLanguageCommandValidator()
    {
        RuleFor(x => x.Language).Must(l => l is "ar" or "en").WithMessage(ErrorCodes.Validation.InvalidValue);
    }
}
