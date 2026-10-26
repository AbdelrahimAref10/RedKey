using MediatR;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Notifications;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Features.ContactMessages.Common;
using RedKey.Domain.Entities;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.ContactMessages.CreateContactMessage;

public sealed class CreateContactMessageCommandHandler : IRequestHandler<CreateContactMessageCommand, Result<Guid>>
{
    /// <summary>Hub event of a new inbox message (the admin chat icon listens to it).</summary>
    public const string ContactMessageEvent = "contactMessage";

    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;

    public CreateContactMessageCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        INotificationService notifications)
    {
        _context = context;
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task<Result<Guid>> Handle(CreateContactMessageCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result<Guid>.Failure(ErrorCodes.Units.Forbidden, 403);

        var message = ContactMessage.Create(
            userId,
            request.Name.Trim(),
            request.CountryIso.Trim().ToUpperInvariant(),
            PhoneDigits.DialCode(request.DialCode),
            PhoneDigits.National(request.PhoneNumber),
            (ContactSubject)request.Subject,
            request.Message.Trim());

        _context.ContactMessages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        // Straight to the admins' chat icon (live + sound). Admin dashboards are Arabic-first.
        await _notifications.PushToSuperAdminsAsync(ContactMessageEvent, message.ToListItem(ar: true), cancellationToken);

        return Result<Guid>.Success(message.Id);
    }
}
