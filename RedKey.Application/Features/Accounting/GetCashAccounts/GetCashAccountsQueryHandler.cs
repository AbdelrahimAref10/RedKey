using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Accounting;

namespace RedKey.Application.Features.Accounting.GetCashAccounts;

public sealed class GetCashAccountsQueryHandler : IRequestHandler<GetCashAccountsQuery, Result<IReadOnlyList<CashAccountDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILanguageContext _language;

    public GetCashAccountsQueryHandler(IApplicationDbContext context, ILanguageContext language)
    {
        _context = context;
        _language = language;
    }

    public async Task<Result<IReadOnlyList<CashAccountDto>>> Handle(GetCashAccountsQuery request, CancellationToken cancellationToken)
    {
        var ar = _language.IsArabic;

        // Cash accounts are assets: balance = debit − credit.
        var accounts = await _context.Accounts.AsNoTracking()
            .Where(a => a.IsCash && a.IsActive)
            .OrderBy(a => a.Code)
            .Select(a => new CashAccountDto
            {
                Id = a.Id,
                Code = a.Code,
                Name = ar ? a.NameAr : a.NameEn,
                Balance = _context.JournalLines.Where(l => l.AccountId == a.Id).Sum(l => (decimal?)(l.Debit - l.Credit)) ?? 0
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<CashAccountDto>>.Success(accounts);
    }
}
