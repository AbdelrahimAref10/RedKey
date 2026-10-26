using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Accounting;

namespace RedKey.Application.Features.Accounting.GetJournalEntries;

/// <summary>The journal, newest first; <paramref name="SourceType"/> filters by operation (viewing fee...).</summary>
public sealed record GetJournalEntriesQuery(int? SourceType, int? Page, int? PageSize)
    : IRequest<Result<PagedResult<JournalEntryDto>>>;
