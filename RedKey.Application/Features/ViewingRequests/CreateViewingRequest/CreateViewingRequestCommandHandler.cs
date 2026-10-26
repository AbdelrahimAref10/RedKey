using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Notifications;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Features.ViewingRequests.Common;
using RedKey.Domain.Entities;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.ViewingRequests.CreateViewingRequest;

public sealed class CreateViewingRequestCommandHandler : IRequestHandler<CreateViewingRequestCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;

    public CreateViewingRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        INotificationService notifications)
    {
        _context = context;
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task<Result<Guid>> Handle(CreateViewingRequestCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } customerId)
            return Result<Guid>.Failure(ErrorCodes.Units.Forbidden, 403);

        var unit = await _context.Units.AsNoTracking()
            .Where(u => u.Id == request.UnitId && u.Status == UnitStatus.Approved)
            .Select(u => new { u.Code, u.TitleAr, u.TitleEn, u.ViewingFee })
            .FirstOrDefaultAsync(cancellationToken);
        if (unit is null)
            return Result<Guid>.NotFound(ErrorCodes.Units.NotAvailable);

        var viewing = ViewingRequest.Create(
            request.UnitId,
            customerId,
            request.Name.Trim(),
            request.PhoneNumber.Trim(),
            (PreferredTime)request.PreferredTime,
            unit.ViewingFee);

        _context.ViewingRequests.Add(viewing);
        await _context.SaveChangesAsync(cancellationToken);

        // Every super admin gets it live (bell + sound); the link opens it in the requests table.
        await _notifications.NotifySuperAdminsAsync(
            ViewingRequestNotifications.Requested(viewing.Id, viewing.Name, unit.Code, unit.TitleAr, unit.TitleEn),
            cancellationToken);

        return Result<Guid>.Success(viewing.Id);
    }
}
