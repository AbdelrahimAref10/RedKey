using MediatR;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Authentication;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Dtos.Auth;
using RedKey.Application.Features.Auth;

namespace RedKey.Application.Features.Account.UpdateMyLanguage;

public sealed class UpdateMyLanguageCommandHandler : IRequestHandler<UpdateMyLanguageCommand, Result<AuthResponseDto>>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public UpdateMyLanguageCommandHandler(
        IIdentityService identityService,
        ICurrentUserService currentUser,
        IJwtTokenGenerator tokenGenerator)
    {
        _identityService = identityService;
        _currentUser = currentUser;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthResponseDto>> Handle(UpdateMyLanguageCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result<AuthResponseDto>.Failure(ErrorCodes.Auth.InvalidCredentials, 401);

        var updated = await _identityService.SetPreferredLanguageAsync(userId, request.Language, cancellationToken);
        if (!updated.IsSuccess)
            return Result<AuthResponseDto>.Failure(updated.Error, updated.StatusCode);

        return Result<AuthResponseDto>.Success(AuthResponseFactory.Create(updated.Value!, _tokenGenerator));
    }
}
