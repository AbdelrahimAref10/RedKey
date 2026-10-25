using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Authentication;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Dtos.Auth;

namespace RedKey.Application.Features.Auth.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginCommandHandler(IIdentityService identityService, IJwtTokenGenerator tokenGenerator)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var signIn = await _identityService.SignInAsync(request.Identifier.Trim(), request.Password, cancellationToken);

        if (!signIn.IsSuccess)
            return Result<AuthResponseDto>.Failure(signIn.Error, signIn.StatusCode);

        return Result<AuthResponseDto>.Success(AuthResponseFactory.Create(signIn.Value!, _tokenGenerator));
    }
}
