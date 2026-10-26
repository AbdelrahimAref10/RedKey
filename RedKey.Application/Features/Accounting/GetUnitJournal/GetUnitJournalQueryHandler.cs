using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Accounting;
using RedKey.Application.Features.Accounting.Common;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Accounting.GetUnitJournal;

public sealed class GetUnitJournalQueryHandler : IRequestHandler<GetUnitJournalQuery, Result<UnitJournalDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILanguageContext _language;

    public GetUnitJournalQueryHandler(IApplicationDbContext context, ILanguageContext language)
    {
        _context = context;
        _language = language;
    }

    public async Task<Result<UnitJournalDto>> Handle(GetUnitJournalQuery request, CancellationToken cancellationToken)
    {
        var entries = await _context.JournalEntries.AsNoTracking()
            .Where(e => e.UnitId == request.UnitId)
            .OrderByDescending(e => e.EntryDateUtc).ThenByDescending(e => e.Number)
            .ToDtosAsync(_context, _language.IsArabic, cancellationToken);

        // Revenue grows on the credit side: credit − debit (a reversal cancels its original).
        var netRevenue = await _context.JournalLines.AsNoTracking()
            .Where(l => l.Account.Type == AccountType.Revenue)
            .Where(l => _context.JournalEntries.Any(e => e.Id == l.EntryId && e.UnitId == request.UnitId))
            .SumAsync(l => (decimal?)(l.Credit - l.Debit), cancellationToken) ?? 0;

        return Result<UnitJournalDto>.Success(new UnitJournalDto { Entries = entries, NetRevenue = netRevenue });
    }
}
