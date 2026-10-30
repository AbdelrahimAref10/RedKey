using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Persistence;
using RedKey.Domain.Entities;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Favorites.AddFavorite;

public sealed class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AddFavoriteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure(ErrorCodes.Units.Forbidden, 403);

        if (await _context.Favorites.AnyAsync(f => f.CustomerUserId == userId && f.UnitId == request.UnitId, cancellationToken))
            return Result.Success();

        var published = await _context.Units.AnyAsync(
            u => u.Id == request.UnitId && u.Status == UnitStatus.Approved, cancellationToken);
        if (!published)
            return Result.NotFound(ErrorCodes.Units.NotAvailable);

        _context.Favorites.Add(Favorite.Create(userId, request.UnitId));
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Two taps at the same time (web + mobile): the other request saved it already.
        }

        return Result.Success();
    }
}
