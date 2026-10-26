using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Notifications;

namespace RedKey.Application.Features.Notifications.GetMyNotifications;

public sealed class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Result<NotificationsPageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILanguageContext _language;

    public GetMyNotificationsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, ILanguageContext language)
    {
        _context = context;
        _currentUser = currentUser;
        _language = language;
    }

    public async Task<Result<NotificationsPageDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result<NotificationsPageDto>.Failure(ErrorCodes.Notifications.NotFound, 401);

        var paging = Paging.From(request.Page, request.PageSize);
        var ar = _language.IsArabic;
        var mine = _context.Notifications.AsNoTracking().Where(n => n.RecipientUserId == userId);

        var total = await mine.CountAsync(cancellationToken);
        var unread = await mine.CountAsync(n => !n.IsRead, cancellationToken);
        var items = await mine
            .OrderByDescending(n => n.CreatedAtUtc)
            .Skip(paging.Skip).Take(paging.PageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = (int)n.Type,
                Title = ar ? n.TitleAr : n.TitleEn,
                Body = ar ? n.BodyAr : n.BodyEn,
                LinkUrl = n.LinkUrl,
                IsRead = n.IsRead,
                CreatedAtUtc = n.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return Result<NotificationsPageDto>.Success(new NotificationsPageDto
        {
            Page = new PagedResult<NotificationDto>
            {
                Items = items,
                Page = paging.Page,
                PageSize = paging.PageSize,
                TotalCount = total
            },
            UnreadCount = unread
        });
    }
}
