using MediatR;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Notifications;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Contracts.Storage;
using RedKey.Application.Features.Units.Common;
using RedKey.Domain.Entities.Units;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Units.CreateUnit;

public sealed class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorage _storage;
    private readonly INotificationService _notifications;

    public CreateUnitCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IFileStorage storage,
        INotificationService notifications)
    {
        _context = context;
        _currentUser = currentUser;
        _storage = storage;
        _notifications = notifications;
    }

    public async Task<Result<Guid>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser is not { UserId: { } userId, Role: UserRole.SuperAdmin or UserRole.Broker } user)
            return Result<Guid>.Failure(ErrorCodes.Units.Forbidden, 403);

        var (lookupError, cityId) = await UnitLookupGuard.CheckAsync(_context, request.UnitTypeId, request.AreaId, cancellationToken);
        if (lookupError is not null)
            return Result<Guid>.Failure(lookupError);

        var unit = PropertyUnit.Create(request.ToContent(cityId), userId, user.Role!.Value);

        var images = new UnitImageWriter(_storage, unit.Id, []);
        var cover = await images.ResolveAsync(request.CoverImage, cancellationToken);
        var gallery = cover.IsSuccess
            ? await images.ResolveManyAsync(request.GalleryImages, cancellationToken)
            : Result<List<string>>.Failure(cover.Error);

        if (!gallery.IsSuccess)
        {
            images.Rollback();
            return Result<Guid>.Failure(gallery.Error);
        }

        unit.SetCoverImage(cover.Value!);
        unit.SetGallery(gallery.Value!);
        unit.SetPaymentPlans(request.ToPaymentPlans());
        unit.SetFeatures(request.ToFeatures());
        if (user.Role == UserRole.SuperAdmin)
            unit.SetViewingFee(request.ViewingFee ?? 0);

        _context.Units.Add(unit);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            images.Rollback();
            throw;
        }

        if (unit.Status == UnitStatus.PendingReview)
            await _notifications.NotifySuperAdminsAsync(
                UnitNotifications.Submitted(unit, user.FullName ?? string.Empty, isEdit: false), cancellationToken);

        return Result<Guid>.Success(unit.Id);
    }
}
