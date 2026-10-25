using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Auth;

namespace RedKey.Application.Features.Auth.Login;

/// <param name="Identifier">Email or phone number.</param>
public sealed record LoginCommand(string Identifier, string Password) : IRequest<Result<AuthResponseDto>>;
