using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Notifications;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Contracts.Storage;
using RedKey.Application.Features.Units.Common;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Units.UpdateUnit;

public sealed class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorage _storage;
    private readonly INotificationService _notifications;

    public UpdateUnitCommandHandler(
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

    public async Task<Result> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser is not { UserId: { } userId, Role: UserRole.SuperAdmin or UserRole.Broker } user)
            return Result.Failure(ErrorCodes.Units.Forbidden, 403);

        var unit = await _context.Units
            .Include(u => u.Details)
            .Include(u => u.Images)
            .Include(u => u.PaymentPlans)
            .Include(u => u.Features)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        // A broker only sees (and edits) his own units.
        var isBroker = user.Role == UserRole.Broker;
        if (unit is null || (isBroker && unit.CreatedByUserId != userId))
            return Result.NotFound(ErrorCodes.Units.NotFound);

        var (lookupError, cityId) = await UnitLookupGuard.CheckAsync(_context, request.UnitTypeId, request.AreaId, cancellationToken);
        if (lookupError is not null)
            return Result.Failure(lookupError);

        var previousUrls = unit.Images.Select(i => i.Url).Append(unit.CoverImageUrl).ToList();
        var images = new UnitImageWriter(_storage, unit.Id, previousUrls);

        var cover = await images.ResolveAsync(request.CoverImage, cancellationToken);
        var gallery = cover.IsSuccess
            ? await images.ResolveManyAsync(request.GalleryImages, cancellationToken)
            : Result<List<string>>.Failure(cover.Error);

        if (!gallery.IsSuccess)
        {
            images.Rollback();
            return Result.Failure(gallery.Error);
        }

        unit.SetContent(request.ToContent(cityId));
        unit.SetCoverImage(cover.Value!);
        unit.SetGallery(gallery.Value!);
        unit.SetPaymentPlans(request.ToPaymentPlans());
        unit.SetFeatures(request.ToFeatures());
        // Only the super admin prices the viewing; a broker edit keeps it.
        if (!isBroker && request.ViewingFee is { } fee)
            unit.SetViewingFee(fee);

        if (isBroker)
            unit.SubmitForReview();

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            images.Rollback();
            throw;
        }

        images.DeleteUnused(gallery.Value!.Append(cover.Value!));

        if (isBroker)
            await _notifications.NotifySuperAdminsAsync(
                UnitNotifications.Submitted(unit, user.FullName ?? string.Empty, isEdit: true), cancellationToken);

        return Result.Success();
    }
}
