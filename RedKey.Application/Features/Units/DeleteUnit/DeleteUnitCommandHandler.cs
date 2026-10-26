using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Persistence;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Units.DeleteUnit;

public sealed class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteUnitCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser is not { UserId: { } userId, Role: UserRole.SuperAdmin or UserRole.Broker } user)
            return Result.Failure(ErrorCodes.Units.Forbidden, 403);

        var unit = await _context.Units.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (unit is null || (user.Role == UserRole.Broker && unit.CreatedByUserId != userId))
            return Result.NotFound(ErrorCodes.Units.NotFound);

        unit.Delete();
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
