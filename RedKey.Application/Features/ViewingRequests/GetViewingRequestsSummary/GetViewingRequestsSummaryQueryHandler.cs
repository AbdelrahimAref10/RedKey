using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.ViewingRequests;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.ViewingRequests.GetViewingRequestsSummary;

public sealed class GetViewingRequestsSummaryQueryHandler
    : IRequestHandler<GetViewingRequestsSummaryQuery, Result<ViewingRequestsSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetViewingRequestsSummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ViewingRequestsSummaryDto>> Handle(
        GetViewingRequestsSummaryQuery request,
        CancellationToken cancellationToken)
    {
        // One GROUP BY instead of a count per status.
        var counts = await _context.ViewingRequests.AsNoTracking()
            .GroupBy(v => v.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);

        int Of(ViewingRequestStatus status) => counts.GetValueOrDefault(status);

        return Result<ViewingRequestsSummaryDto>.Success(new ViewingRequestsSummaryDto
        {
            Total = counts.Values.Sum(),
            New = Of(ViewingRequestStatus.New),
            Contacted = Of(ViewingRequestStatus.Contacted),
            Closed = Of(ViewingRequestStatus.Closed),
            Completed = Of(ViewingRequestStatus.Completed)
        });
    }
}
