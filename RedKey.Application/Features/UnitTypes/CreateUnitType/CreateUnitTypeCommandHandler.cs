using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Contracts.Storage;
using RedKey.Domain.Entities;

namespace RedKey.Application.Features.UnitTypes.CreateUnitType;

public sealed class CreateUnitTypeCommandHandler : IRequestHandler<CreateUnitTypeCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _storage;

    public CreateUnitTypeCommandHandler(IApplicationDbContext context, IFileStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<Result<int>> Handle(CreateUnitTypeCommand request, CancellationToken cancellationToken)
    {
        var type = UnitType.Create(request.NameAr.Trim(), request.NameEn.Trim(), request.SortOrder);

        if (!string.IsNullOrWhiteSpace(request.Icon))
        {
            var icon = await _storage.SaveBase64ImageAsync("unit-types", request.Icon, allowSvg: true, cancellationToken);
            if (!icon.IsSuccess)
                return Result<int>.Failure(icon.Error);
            type.SetIcon(icon.Value);
        }

        _context.UnitTypes.Add(type);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(type.Id);
    }
}
