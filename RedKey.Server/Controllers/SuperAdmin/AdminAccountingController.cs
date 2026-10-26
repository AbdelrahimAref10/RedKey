using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Accounting;
using RedKey.Application.Features.Accounting.GetAccountingOverview;
using RedKey.Application.Features.Accounting.GetCashAccounts;
using RedKey.Application.Features.Accounting.GetJournalEntries;
using RedKey.Application.Features.Accounting.GetTreasuryMovements;
using RedKey.Application.Features.Accounting.GetUnitJournal;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.SuperAdmin;

/// <summary>The company books (double-entry journal): treasury, entries, profit.</summary>
[Route("api/admin/accounting")]
public sealed class AdminAccountingController : SuperAdminControllerBase
{
    private readonly ISender _sender;

    public AdminAccountingController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Treasury balances + revenue / expenses / net profit of a period (default: this month).</summary>
    [HttpGet("overview")]
    [ProducesResponseType<AccountingOverviewDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverview([FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetAccountingOverviewQuery(fromUtc, toUtc), cancellationToken)).ToActionResult();

    /// <summary>Cash / bank accounts with balances ("received in" picker).</summary>
    [HttpGet("cash-accounts")]
    [ProducesResponseType<IReadOnlyList<CashAccountDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCashAccounts(CancellationToken cancellationToken) =>
        (await _sender.Send(new GetCashAccountsQuery(), cancellationToken)).ToActionResult();

    [HttpGet("treasury")]
    [ProducesResponseType<PagedResult<TreasuryMovementDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTreasury([FromQuery] int? accountId, [FromQuery] int? page, [FromQuery] int? pageSize, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetTreasuryMovementsQuery(accountId, page, pageSize), cancellationToken)).ToActionResult();

    [HttpGet("journal")]
    [ProducesResponseType<PagedResult<JournalEntryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJournal([FromQuery] int? sourceType, [FromQuery] int? page, [FromQuery] int? pageSize, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetJournalEntriesQuery(sourceType, page, pageSize), cancellationToken)).ToActionResult();

    /// <summary>The entries of one unit (the "entries" button on the unit page).</summary>
    [HttpGet("units/{unitId:guid}/journal")]
    [ProducesResponseType<UnitJournalDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnitJournal(Guid unitId, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetUnitJournalQuery(unitId), cancellationToken)).ToActionResult();
}
