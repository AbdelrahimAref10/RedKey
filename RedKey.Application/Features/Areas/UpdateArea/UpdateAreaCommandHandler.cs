using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;

namespace RedKey.Application.Features.Areas.UpdateArea;

public sealed class UpdateAreaCommandHandler : IRequestHandler<UpdateAreaCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateAreaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateAreaCommand request, CancellationToken cancellationToken)
    {
        var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (area is null)
            return Result.NotFound(ErrorCodes.Lookups.AreaNotFound);

        area.Update(request.NameAr.Trim(), request.NameEn.Trim(), request.SortOrder, request.IsActive);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
