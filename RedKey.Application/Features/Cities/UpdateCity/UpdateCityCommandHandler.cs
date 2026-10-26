using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;

namespace RedKey.Application.Features.Cities.UpdateCity;

public sealed class UpdateCityCommandHandler : IRequestHandler<UpdateCityCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateCityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (city is null)
            return Result.NotFound(ErrorCodes.Lookups.CityNotFound);

        city.Update(request.NameAr.Trim(), request.NameEn.Trim(), request.SortOrder, request.IsActive);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
