using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;
using RedKey.Domain.Entities;

namespace RedKey.Application.Features.Cities.CreateCity;

public sealed class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreateCityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        var city = City.Create(request.NameAr.Trim(), request.NameEn.Trim(), request.SortOrder);
        _context.Cities.Add(city);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(city.Id);
    }
}
