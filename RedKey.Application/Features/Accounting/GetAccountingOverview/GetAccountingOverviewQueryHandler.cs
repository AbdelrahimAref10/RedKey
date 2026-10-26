using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Accounting;
using RedKey.Application.Features.Accounting.GetCashAccounts;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Accounting.GetAccountingOverview;

public sealed class GetAccountingOverviewQueryHandler : IRequestHandler<GetAccountingOverviewQuery, Result<AccountingOverviewDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;
    private readonly TimeProvider _time;

    public GetAccountingOverviewQueryHandler(IApplicationDbContext context, ISender sender, TimeProvider time)
    {
        _context = context;
        _sender = sender;
        _time = time;
    }

    public async Task<Result<AccountingOverviewDto>> Handle(GetAccountingOverviewQuery request, CancellationToken cancellationToken)
    {
        var now = _time.GetUtcNow().UtcDateTime;
        var from = request.FromUtc ?? new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = request.ToUtc ?? from.AddMonths(1);

        var cash = (await _sender.Send(new GetCashAccountsQuery(), cancellationToken)).Value!;

        // Totals per account type in the period; the type decides the sign (see AccountType.Balance).
        var totals = await _context.JournalLines.AsNoTracking()
            .Where(l => l.Account.Type == AccountType.Revenue || l.Account.Type == AccountType.Expense)
            .Where(l => _context.JournalEntries.Any(e => e.Id == l.EntryId && e.EntryDateUtc >= from && e.EntryDateUtc < to))
            .GroupBy(l => l.Account.Type)
            .Select(g => new { Type = g.Key, Debit = g.Sum(l => l.Debit), Credit = g.Sum(l => l.Credit) })
            .ToListAsync(cancellationToken);

        decimal Of(AccountType type) =>
            totals.Where(t => t.Type == type).Select(t => type.Balance(t.Debit, t.Credit)).FirstOrDefault();

        var revenue = Of(AccountType.Revenue);
        var expenses = Of(AccountType.Expense);

        var trial = await _context.JournalLines.AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new { Debit = g.Sum(l => l.Debit), Credit = g.Sum(l => l.Credit) })
            .FirstOrDefaultAsync(cancellationToken);

        return Result<AccountingOverviewDto>.Success(new AccountingOverviewDto
        {
            CashAccounts = cash,
            TotalCash = cash.Sum(a => a.Balance),
            FromUtc = from,
            ToUtc = to,
            Revenue = revenue,
            Expenses = expenses,
            NetProfit = revenue - expenses,
            IsBalanced = trial is null || trial.Debit == trial.Credit
        });
    }
}
