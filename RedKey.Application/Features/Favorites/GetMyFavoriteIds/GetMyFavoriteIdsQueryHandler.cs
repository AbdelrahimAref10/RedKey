using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Persistence;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Favorites.GetMyFavoriteIds;

public sealed class GetMyFavoriteIdsQueryHandler : IRequestHandler<GetMyFavoriteIdsQuery, Result<IReadOnlyList<Guid>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyFavoriteIdsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<Guid>>> Handle(GetMyFavoriteIdsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Result<IReadOnlyList<Guid>>.Failure(ErrorCodes.Units.Forbidden, 403);

        var ids = await _context.Favorites.AsNoTracking()
            .Where(f => f.CustomerUserId == userId && f.Unit.Status == UnitStatus.Approved)
            .OrderByDescending(f => f.CreatedAtUtc)
            .Select(f => f.UnitId)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<Guid>>.Success(ids);
    }
}
