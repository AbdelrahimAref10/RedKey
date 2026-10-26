using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;
using RedKey.Domain.Entities;

namespace RedKey.Application.Features.Areas.CreateArea;

public sealed class CreateAreaCommandHandler : IRequestHandler<CreateAreaCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreateAreaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateAreaCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Cities.AnyAsync(c => c.Id == request.CityId, cancellationToken))
            return Result<int>.NotFound(ErrorCodes.Lookups.CityNotFound);

        var area = Area.Create(request.CityId, request.NameAr.Trim(), request.NameEn.Trim(), request.SortOrder);
        _context.Areas.Add(area);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(area.Id);
    }
}
