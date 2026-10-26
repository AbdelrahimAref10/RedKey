using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Accounting;

namespace RedKey.Application.Features.Accounting.GetUnitJournal;

/// <summary>Every journal entry of one unit (with reversals) and the net revenue it brought.</summary>
public sealed record GetUnitJournalQuery(Guid UnitId) : IRequest<Result<UnitJournalDto>>;
