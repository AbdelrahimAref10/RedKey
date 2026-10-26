using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Notifications;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Features.Units.Common;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Units.ReviewUnit;

public sealed class ReviewUnitCommandHandler : IRequestHandler<ReviewUnitCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;
    private readonly TimeProvider _timeProvider;

    public ReviewUnitCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        INotificationService notifications,
        TimeProvider timeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _notifications = notifications;
        _timeProvider = timeProvider;
    }

    public async Task<Result> Handle(ReviewUnitCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser is not { UserId: { } reviewerId, Role: UserRole.SuperAdmin })
            return Result.Failure(ErrorCodes.Units.Forbidden, 403);

        var unit = await _context.Units.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (unit is null)
            return Result.NotFound(ErrorCodes.Units.NotFound);

        unit.Review((UnitReviewDecision)request.Decision, request.Note, reviewerId, _timeProvider.GetUtcNow().UtcDateTime);
        await _context.SaveChangesAsync(cancellationToken);

        if (unit.CreatedByRole == UserRole.Broker)
            await _notifications.NotifyUserAsync(unit.CreatedByUserId, UnitNotifications.Reviewed(unit), cancellationToken);

        return Result.Success();
    }
}
