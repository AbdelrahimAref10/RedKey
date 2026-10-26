using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;

namespace RedKey.Application.Features.ContactMessages.DeleteContactMessage;

public sealed class DeleteContactMessageCommandHandler : IRequestHandler<DeleteContactMessageCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteContactMessageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteContactMessageCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _context.ContactMessages
            .Where(m => m.Id == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted == 0 ? Result.NotFound(ErrorCodes.ContactMessages.NotFound) : Result.Success();
    }
}
