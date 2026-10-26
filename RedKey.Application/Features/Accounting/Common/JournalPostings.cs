using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Contracts.Persistence;
using RedKey.Domain.Entities;
using RedKey.Domain.Entities.Accounting;
using RedKey.Domain.Entities.Units;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Accounting.Common;

/// <summary>
/// The posting rules: which accounts each business operation debits and credits.
/// Callers add the entry and save it in the same SaveChanges as the business change (one transaction).
/// </summary>
internal static class JournalPostings
{
    /// <summary>
    /// Viewing done → the fee was collected:
    /// Dr cash / bank (asset grows) — Cr viewing fee revenue (revenue grows).
    /// </summary>
    public static async Task<(string? Error, JournalEntry? Entry)> ViewingFeeAsync(
        IApplicationDbContext context,
        ViewingRequest viewing,
        PropertyUnit unit,
        int? cashAccountId,
        Guid userId,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        var cash = cashAccountId is { } id
            ? await context.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.IsCash && a.IsActive, cancellationToken)
            : await context.Accounts.FirstOrDefaultAsync(a => a.Code == SystemAccounts.Cash, cancellationToken);
        if (cash is null)
            return (ErrorCodes.Accounting.InvalidCashAccount, null);

        var revenue = await context.Accounts.SingleAsync(a => a.Code == SystemAccounts.ViewingFeeRevenue, cancellationToken);

        var entry = JournalEntry.Create(
            JournalSourceType.ViewingFee,
            viewing.Id,
            unit.Id,
            $"رسوم معاينة {unit.Code} · {viewing.Name}",
            $"Viewing fee {unit.Code} · {viewing.Name}",
            userId,
            nowUtc,
            [
                new JournalLineInput(cash.Id, viewing.ViewingFee, 0),
                new JournalLineInput(revenue.Id, 0, viewing.ViewingFee)
            ]);

        context.JournalEntries.Add(entry);
        return (null, entry);
    }

    /// <summary>Cancels the active posting of a business record with a reversing entry (nothing is ever deleted).</summary>
    public static async Task ReverseAsync(
        IApplicationDbContext context,
        JournalSourceType sourceType,
        Guid sourceId,
        string reasonAr,
        string reasonEn,
        Guid userId,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        var original = await context.JournalEntries
            .Include(e => e.Lines)
            .FirstOrDefaultAsync(e =>
                e.SourceType == sourceType && e.SourceId == sourceId && e.ReversalOfId == null && !e.IsReversed,
                cancellationToken);
        if (original is null)
            return;

        context.JournalEntries.Add(original.Reverse(
            $"عكس قيد {original.Number} · {reasonAr}",
            $"Reversal of {original.Number} · {reasonEn}",
            userId,
            nowUtc));
    }
}
