using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.Accounting;
using RedKey.Application.Features.Accounting.Common;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Accounting.GetJournalEntries;

public sealed class GetJournalEntriesQueryHandler : IRequestHandler<GetJournalEntriesQuery, Result<PagedResult<JournalEntryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILanguageContext _language;

    public GetJournalEntriesQueryHandler(IApplicationDbContext context, ILanguageContext language)
    {
        _context = context;
        _language = language;
    }

    public async Task<Result<PagedResult<JournalEntryDto>>> Handle(GetJournalEntriesQuery request, CancellationToken cancellationToken)
    {
        var paging = Paging.From(request.Page, request.PageSize);
        var query = _context.JournalEntries.AsNoTracking();
        if (request.SourceType is { } source)
            query = query.Where(e => e.SourceType == (JournalSourceType)source);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(e => e.EntryDateUtc).ThenByDescending(e => e.Number)
            .Skip(paging.Skip).Take(paging.PageSize)
            .ToDtosAsync(_context, _language.IsArabic, cancellationToken);

        return Result<PagedResult<JournalEntryDto>>.Success(new PagedResult<JournalEntryDto>
        {
            Items = items,
            Page = paging.Page,
            PageSize = paging.PageSize,
            TotalCount = total
        });
    }
}
