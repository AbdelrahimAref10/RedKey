using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Auth;

namespace RedKey.Application.Features.Account.UpdateMyLanguage;

/// <summary>
/// Saves the session language of the signed-in user (admin / broker) and returns a fresh token
/// carrying the new <c>lang</c> claim.
/// </summary>
public sealed record UpdateMyLanguageCommand(string Language) : IRequest<Result<AuthResponseDto>>;
