using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Contracts.Storage;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.UnitTypes.DeleteUnitType;

public sealed class DeleteUnitTypeCommandHandler : IRequestHandler<DeleteUnitTypeCommand, Result<LookupDeleteResultDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _storage;

    public DeleteUnitTypeCommandHandler(IApplicationDbContext context, IFileStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<Result<LookupDeleteResultDto>> Handle(DeleteUnitTypeCommand request, CancellationToken cancellationToken)
    {
        var type = await _context.UnitTypes.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (type is null)
            return Result<LookupDeleteResultDto>.NotFound(ErrorCodes.Lookups.UnitTypeNotFound);

        // Only live units count (the query filter skips deleted ones).
        if (await _context.Units.AnyAsync(u => u.UnitTypeId == type.Id, cancellationToken))
        {
            type.Deactivate();
            await _context.SaveChangesAsync(cancellationToken);
            return Result<LookupDeleteResultDto>.Success(new LookupDeleteResultDto { Deleted = false });
        }

        // Soft delete: deleted units still reference the type. The icon is no longer shown anywhere.
        var icon = type.IconUrl;
        type.SetIcon(null);
        type.Delete();
        await _context.SaveChangesAsync(cancellationToken);
        _storage.Delete(icon);

        return Result<LookupDeleteResultDto>.Success(new LookupDeleteResultDto { Deleted = true });
    }
}
