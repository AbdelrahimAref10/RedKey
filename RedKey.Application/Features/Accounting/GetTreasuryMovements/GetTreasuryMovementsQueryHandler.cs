using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Accounting;

namespace RedKey.Application.Features.Accounting.GetTreasuryMovements;

public sealed class GetTreasuryMovementsQueryHandler
    : IRequestHandler<GetTreasuryMovementsQuery, Result<PagedResult<TreasuryMovementDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILanguageContext _language;

    public GetTreasuryMovementsQueryHandler(IApplicationDbContext context, ILanguageContext language)
    {
        _context = context;
        _language = language;
    }

    public async Task<Result<PagedResult<TreasuryMovementDto>>> Handle(
        GetTreasuryMovementsQuery request,
        CancellationToken cancellationToken)
    {
        var paging = Paging.From(request.Page, request.PageSize);
        var ar = _language.IsArabic;

        var lines = _context.JournalLines.AsNoTracking().Where(l => l.Account.IsCash);
        if (request.AccountId is { } accountId)
            lines = lines.Where(l => l.AccountId == accountId);

        var movements =
            from l in lines
            join e in _context.JournalEntries on l.EntryId equals e.Id
            select new { l, e };

        var total = await movements.CountAsync(cancellationToken);
        var items = await movements
            .OrderByDescending(x => x.e.EntryDateUtc).ThenByDescending(x => x.l.Id)
            .Skip(paging.Skip).Take(paging.PageSize)
            .Select(x => new TreasuryMovementDto
            {
                LineId = x.l.Id,
                EntryId = x.e.Id,
                EntryNumber = x.e.Number,
                EntryDateUtc = x.e.EntryDateUtc,
                Description = ar ? x.e.DescriptionAr : x.e.DescriptionEn,
                AccountName = ar ? x.l.Account.NameAr : x.l.Account.NameEn,
                In = x.l.Debit,
                Out = x.l.Credit
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<TreasuryMovementDto>>.Success(new PagedResult<TreasuryMovementDto>
        {
            Items = items,
            Page = paging.Page,
            PageSize = paging.PageSize,
            TotalCount = total
        });
    }
}
