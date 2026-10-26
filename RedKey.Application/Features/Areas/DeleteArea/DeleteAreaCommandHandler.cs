using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Areas.DeleteArea;

public sealed class DeleteAreaCommandHandler : IRequestHandler<DeleteAreaCommand, Result<LookupDeleteResultDto>>
{
    private readonly IApplicationDbContext _context;

    public DeleteAreaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<LookupDeleteResultDto>> Handle(DeleteAreaCommand request, CancellationToken cancellationToken)
    {
        var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (area is null)
            return Result<LookupDeleteResultDto>.NotFound(ErrorCodes.Lookups.AreaNotFound);

        // Only live units count (the query filter skips deleted ones). Deleting is a soft delete,
        // because deleted units still reference the area.
        var inUse = await _context.Units.AnyAsync(u => u.AreaId == area.Id, cancellationToken);
        if (inUse)
            area.Deactivate();
        else
            area.Delete();

        await _context.SaveChangesAsync(cancellationToken);
        return Result<LookupDeleteResultDto>.Success(new LookupDeleteResultDto { Deleted = !inUse });
    }
}
