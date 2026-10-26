using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Persistence;

namespace RedKey.Application.Features.Notifications.MarkNotificationsRead;

public sealed class MarkNotificationsReadCommandHandler : IRequestHandler<MarkNotificationsReadCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly TimeProvider _timeProvider;

    public MarkNotificationsReadCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, TimeProvider timeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
    }

    public async Task<Result> Handle(MarkNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure(ErrorCodes.Notifications.NotFound, 401);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var unread = _context.Notifications.Where(n => n.RecipientUserId == userId && !n.IsRead);

        if (request.Id is { } id)
            unread = unread.Where(n => n.Id == id);

        // Single UPDATE statement, no entity loading.
        await unread.ExecuteUpdateAsync(s => s
            .SetProperty(n => n.IsRead, true)
            .SetProperty(n => n.ReadAtUtc, now), cancellationToken);

        return Result.Success();
    }
}
