using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Auth;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Auth.Register;

/// <summary>
/// Public sign-up. Only <see cref="UserRole.Broker"/> and <see cref="UserRole.Customer"/> are accepted.
/// </summary>
public sealed record RegisterCommand : IRequest<Result<AuthResponseDto>>
{
    public required UserRole Role { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
    public string? City { get; init; }
    public string PreferredLanguage { get; init; } = "ar";

    // Broker only
    public BrokerType? BrokerType { get; init; }
    public string? CompanyName { get; init; }
    public string? NationalId { get; init; }
}
