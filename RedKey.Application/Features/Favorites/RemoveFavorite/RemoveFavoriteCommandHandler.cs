using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Persistence;

namespace RedKey.Application.Features.Favorites.RemoveFavorite;

public sealed class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RemoveFavoriteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure(ErrorCodes.Units.Forbidden, 403);

        // IgnoreQueryFilters: a favorite of a deleted unit can still be removed.
        await _context.Favorites.IgnoreQueryFilters()
            .Where(f => f.CustomerUserId == userId && f.UnitId == request.UnitId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }
}
