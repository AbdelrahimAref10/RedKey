using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Accounting;
using RedKey.Domain.Entities.Accounting;

namespace RedKey.Application.Features.Accounting.Common;

internal static class JournalProjection
{
    /// <summary>Entries (already filtered / paged) with their lines, accounts, unit codes and reversal links.</summary>
    public static async Task<List<JournalEntryDto>> ToDtosAsync(
        this IQueryable<JournalEntry> entries,
        IApplicationDbContext context,
        bool ar,
        CancellationToken cancellationToken)
    {
        var rows = await entries
            .Select(e => new
            {
                e.Id,
                e.Number,
                e.EntryDateUtc,
                Description = ar ? e.DescriptionAr : e.DescriptionEn,
                e.SourceType,
                e.SourceId,
                e.UnitId,
                UnitCode = context.Units.IgnoreQueryFilters().Where(u => u.Id == e.UnitId).Select(u => u.Code).FirstOrDefault(),
                ReversalOfNumber = context.JournalEntries.Where(o => o.Id == e.ReversalOfId).Select(o => o.Number).FirstOrDefault(),
                e.IsReversed,
                Lines = e.Lines
                    .OrderByDescending(l => l.Debit)
                    .Select(l => new JournalLineDto
                    {
                        AccountCode = l.Account.Code,
                        AccountName = ar ? l.Account.NameAr : l.Account.NameEn,
                        Debit = l.Debit,
                        Credit = l.Credit,
                        Memo = l.Memo
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return rows.Select(r => new JournalEntryDto
        {
            Id = r.Id,
            Number = r.Number,
            EntryDateUtc = r.EntryDateUtc,
            Description = r.Description,
            SourceType = (int)r.SourceType,
            SourceTypeName = EnumNames.Of(r.SourceType, ar),
            SourceId = r.SourceId,
            UnitId = r.UnitId,
            UnitCode = r.UnitCode,
            ReversalOfNumber = r.ReversalOfNumber,
            IsReversed = r.IsReversed,
            Total = r.Lines.Sum(l => l.Debit),
            Lines = r.Lines
        }).ToList();
    }
}
