using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Notifications;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Features.Accounting.Common;
using RedKey.Application.Features.ViewingRequests.Common;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.ViewingRequests.UpdateViewingRequestStatus;

public sealed class UpdateViewingRequestStatusCommandHandler : IRequestHandler<UpdateViewingRequestStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notifications;
    private readonly ICurrentUserService _currentUser;
    private readonly TimeProvider _time;

    public UpdateViewingRequestStatusCommandHandler(
        IApplicationDbContext context,
        INotificationService notifications,
        ICurrentUserService currentUser,
        TimeProvider time)
    {
        _context = context;
        _notifications = notifications;
        _currentUser = currentUser;
        _time = time;
    }

    public async Task<Result> Handle(UpdateViewingRequestStatusCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure(ErrorCodes.Units.Forbidden, 403);

        var viewing = await _context.ViewingRequests
            .Include(v => v.Unit)
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);
        if (viewing is null)
            return Result.NotFound(ErrorCodes.ViewingRequests.NotFound);

        var status = (ViewingRequestStatus)request.Status;
        if (viewing.Status == status)
            return Result.Success();

        var unit = viewing.Unit;
        var now = _time.GetUtcNow().UtcDateTime;

        if (status == ViewingRequestStatus.Completed)
        {
            // The fee is posted with the status change (same SaveChanges = same transaction). Free viewings post nothing.
            Guid? entryId = null;
            if (viewing.ViewingFee > 0)
            {
                var (error, entry) = await JournalPostings.ViewingFeeAsync(
                    _context, viewing, unit, request.CashAccountId, userId, now, cancellationToken);
                if (error is not null)
                    return Result.Failure(error);
                entryId = entry!.Id;
            }
            viewing.Complete(now, entryId);
        }
        else
        {
            // Leaving "viewing done": the posted fee is cancelled by a reversing entry.
            if (viewing.Status == ViewingRequestStatus.Completed && viewing.JournalEntryId is not null)
            {
                await JournalPostings.ReverseAsync(
                    _context, JournalSourceType.ViewingFee, viewing.Id,
                    "إلغاء تمام المعاينة", "Viewing marked not done", userId, now, cancellationToken);
            }
            viewing.SetStatus(status);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _notifications.NotifyUserAsync(
            viewing.CustomerUserId,
            ViewingRequestNotifications.StatusChanged(viewing.Id, unit.Id, status, unit.Code, unit.TitleAr, unit.TitleEn),
            cancellationToken);

        return Result.Success();
    }
}
