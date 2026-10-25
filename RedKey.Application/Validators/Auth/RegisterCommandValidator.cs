using FluentValidation;
using RedKey.Application.Common.Errors;
using RedKey.Application.Features.Auth.Register;
using RedKey.Domain.Enums;

namespace RedKey.Application.Validators.Auth;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private const string PhonePattern = @"^\+?[0-9]{8,15}$";

    public RegisterCommandValidator()
    {
        RuleFor(x => x.Role)
            .Must(r => r is UserRole.Broker or UserRole.Customer)
            .WithMessage(ErrorCodes.Auth.RoleNotAllowed);

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .MaximumLength(150).WithMessage(ErrorCodes.Validation.MaxLength);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .EmailAddress().WithMessage(ErrorCodes.Validation.InvalidEmail)
            .MaximumLength(256).WithMessage(ErrorCodes.Validation.MaxLength);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .Matches(PhonePattern).WithMessage(ErrorCodes.Validation.InvalidPhone);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .MinimumLength(8).WithMessage(ErrorCodes.Validation.PasswordWeak)
            .MaximumLength(128).WithMessage(ErrorCodes.Validation.MaxLength)
            .Matches("[A-Z]").WithMessage(ErrorCodes.Validation.PasswordWeak)
            .Matches("[a-z]").WithMessage(ErrorCodes.Validation.PasswordWeak)
            .Matches("[0-9]").WithMessage(ErrorCodes.Validation.PasswordWeak);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage(ErrorCodes.Validation.PasswordMismatch);

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage(ErrorCodes.Validation.MaxLength);

        RuleFor(x => x.PreferredLanguage)
            .Must(l => l is "ar" or "en").WithMessage(ErrorCodes.Validation.InvalidValue);

        When(x => x.Role == UserRole.Broker, () =>
        {
            RuleFor(x => x.BrokerType)
                .NotNull().WithMessage(ErrorCodes.Validation.Required)
                .IsInEnum().WithMessage(ErrorCodes.Validation.InvalidValue);

            RuleFor(x => x.CompanyName)
                .NotEmpty().When(x => x.BrokerType == BrokerType.Company).WithMessage(ErrorCodes.Validation.Required)
                .MaximumLength(150).WithMessage(ErrorCodes.Validation.MaxLength);

            RuleFor(x => x.NationalId)
                .MaximumLength(20).WithMessage(ErrorCodes.Validation.MaxLength);
        });
    }
}
