using MediatR;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Authentication;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Auth;
using RedKey.Domain.Entities;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Auth.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IIdentityService identityService,
        IJwtTokenGenerator tokenGenerator)
    {
        _context = context;
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (request.Role is not (UserRole.Broker or UserRole.Customer))
            return Result<AuthResponseDto>.Failure(ErrorCodes.Auth.RoleNotAllowed);

        var email = request.Email.Trim();
        var phone = request.PhoneNumber.Trim();
        var fullName = request.FullName.Trim();
        var city = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim();

        if (await _identityService.IsEmailTakenAsync(email, cancellationToken))
            return Result<AuthResponseDto>.Conflict(ErrorCodes.Auth.EmailTaken);

        if (await _identityService.IsPhoneTakenAsync(phone, cancellationToken))
            return Result<AuthResponseDto>.Conflict(ErrorCodes.Auth.PhoneTaken);

        await using var transaction = await _context.BeginTransactionAsync(cancellationToken);

        var created = await _identityService.CreateUserAsync(
            new NewUser(fullName, email, phone, request.Password, request.Role, request.PreferredLanguage),
            cancellationToken);

        if (!created.IsSuccess)
            return Result<AuthResponseDto>.Failure(created.Error, created.StatusCode);

        var user = created.Value!;

        if (request.Role == UserRole.Broker)
        {
            _context.Brokers.Add(Broker.Create(
                user.UserId,
                fullName,
                phone,
                email,
                request.BrokerType ?? BrokerType.Independent,
                request.CompanyName?.Trim(),
                request.NationalId?.Trim(),
                city));
        }
        else
        {
            _context.Customers.Add(Customer.Create(user.UserId, fullName, phone, email, city));
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Result<AuthResponseDto>.Success(AuthResponseFactory.Create(user, _tokenGenerator));
    }
}
