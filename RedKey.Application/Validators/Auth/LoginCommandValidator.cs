using FluentValidation;
using RedKey.Application.Common.Errors;
using RedKey.Application.Features.Auth.Login;

namespace RedKey.Application.Validators.Auth;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .MaximumLength(256).WithMessage(ErrorCodes.Validation.MaxLength);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorCodes.Validation.Required)
            .MaximumLength(128).WithMessage(ErrorCodes.Validation.MaxLength);
    }
}
