using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Contracts.Storage;

namespace RedKey.Application.Features.UnitTypes.UpdateUnitType;

public sealed class UpdateUnitTypeCommandHandler : IRequestHandler<UpdateUnitTypeCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _storage;

    public UpdateUnitTypeCommandHandler(IApplicationDbContext context, IFileStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<Result> Handle(UpdateUnitTypeCommand request, CancellationToken cancellationToken)
    {
        var type = await _context.UnitTypes.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (type is null)
            return Result.NotFound(ErrorCodes.Lookups.UnitTypeNotFound);

        var previousIcon = type.IconUrl;

        if (string.IsNullOrWhiteSpace(request.Icon))
        {
            type.SetIcon(null);
        }
        else if (request.Icon != previousIcon)
        {
            var icon = await _storage.SaveBase64ImageAsync("unit-types", request.Icon, allowSvg: true, cancellationToken);
            if (!icon.IsSuccess)
                return Result.Failure(icon.Error);
            type.SetIcon(icon.Value);
        }

        type.Update(request.NameAr.Trim(), request.NameEn.Trim(), request.SortOrder, request.IsActive);
        await _context.SaveChangesAsync(cancellationToken);

        if (previousIcon is not null && previousIcon != type.IconUrl)
            _storage.Delete(previousIcon);

        return Result.Success();
    }
}
