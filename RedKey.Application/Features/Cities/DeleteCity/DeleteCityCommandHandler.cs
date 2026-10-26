using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Cities.DeleteCity;

public sealed class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, Result<LookupDeleteResultDto>>
{
    private readonly IApplicationDbContext _context;

    public DeleteCityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<LookupDeleteResultDto>> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (city is null)
            return Result<LookupDeleteResultDto>.NotFound(ErrorCodes.Lookups.CityNotFound);

        // Only live units count (the query filter skips deleted ones).
        if (await _context.Units.AnyAsync(u => u.CityId == city.Id, cancellationToken))
        {
            city.Deactivate();
            await _context.SaveChangesAsync(cancellationToken);
            return Result<LookupDeleteResultDto>.Success(new LookupDeleteResultDto { Deleted = false });
        }

        // Soft delete (deleted units still reference it), together with its areas.
        city.Delete();
        var areas = await _context.Areas.Where(a => a.CityId == city.Id).ToListAsync(cancellationToken);
        areas.ForEach(a => a.Delete());

        await _context.SaveChangesAsync(cancellationToken);
        return Result<LookupDeleteResultDto>.Success(new LookupDeleteResultDto { Deleted = true });
    }
}
