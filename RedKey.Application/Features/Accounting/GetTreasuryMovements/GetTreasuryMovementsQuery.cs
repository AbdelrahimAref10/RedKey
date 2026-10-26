using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Accounting;

namespace RedKey.Application.Features.Accounting.GetTreasuryMovements;

/// <summary>Cash / bank movements, newest first. <paramref name="AccountId"/> = one cash account only.</summary>
public sealed record GetTreasuryMovementsQuery(int? AccountId, int? Page, int? PageSize)
    : IRequest<Result<PagedResult<TreasuryMovementDto>>>;
