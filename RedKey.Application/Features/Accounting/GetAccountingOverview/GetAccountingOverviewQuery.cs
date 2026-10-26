using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Accounting;

namespace RedKey.Application.Features.Accounting.GetAccountingOverview;

/// <summary>Treasury balances + revenue, expenses and net profit of a period (default: the current month, UTC).</summary>
public sealed record GetAccountingOverviewQuery(DateTime? FromUtc, DateTime? ToUtc) : IRequest<Result<AccountingOverviewDto>>;
