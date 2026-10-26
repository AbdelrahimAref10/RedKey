using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Accounting;

namespace RedKey.Application.Features.Accounting.GetCashAccounts;

/// <summary>Active cash / bank accounts with their balances.</summary>
public sealed record GetCashAccountsQuery : IRequest<Result<IReadOnlyList<CashAccountDto>>>;
