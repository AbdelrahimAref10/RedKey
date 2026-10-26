namespace RedKey.Application.Dtos.Accounting;

/// <summary>A cash / bank account with its balance (treasury cards, "received in" picker).</summary>
public sealed class CashAccountDto
{
    public required int Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required decimal Balance { get; init; }
}

/// <summary>Accounting page header: treasury balances + the income statement of a period.</summary>
public sealed class AccountingOverviewDto
{
    public required IReadOnlyList<CashAccountDto> CashAccounts { get; init; }
    public required decimal TotalCash { get; init; }
    public required DateTime FromUtc { get; init; }
    public required DateTime ToUtc { get; init; }
    /// <summary>Revenue accounts: credit − debit in the period.</summary>
    public required decimal Revenue { get; init; }
    /// <summary>Expense accounts: debit − credit in the period.</summary>
    public required decimal Expenses { get; init; }
    public required decimal NetProfit { get; init; }
    /// <summary>Total debit = total credit over all the books (trial balance check).</summary>
    public required bool IsBalanced { get; init; }
}

/// <summary>One cash / bank movement (treasury statement).</summary>
public sealed class TreasuryMovementDto
{
    public required long LineId { get; init; }
    public required Guid EntryId { get; init; }
    public required string EntryNumber { get; init; }
    public required DateTime EntryDateUtc { get; init; }
    public required string Description { get; init; }
    public required string AccountName { get; init; }
    /// <summary>Money in (debit on a cash account).</summary>
    public required decimal In { get; init; }
    /// <summary>Money out (credit on a cash account).</summary>
    public required decimal Out { get; init; }
}

public sealed class JournalLineDto
{
    public required string AccountCode { get; init; }
    public required string AccountName { get; init; }
    public required decimal Debit { get; init; }
    public required decimal Credit { get; init; }
    public string? Memo { get; init; }
}

public sealed class JournalEntryDto
{
    public required Guid Id { get; init; }
    public required string Number { get; init; }
    public required DateTime EntryDateUtc { get; init; }
    public required string Description { get; init; }
    public required int SourceType { get; init; }
    public required string SourceTypeName { get; init; }
    public required Guid SourceId { get; init; }
    public Guid? UnitId { get; init; }
    public string? UnitCode { get; init; }
    /// <summary>This entry cancels <see cref="ReversalOfNumber"/>.</summary>
    public string? ReversalOfNumber { get; init; }
    /// <summary>A later entry cancelled this one.</summary>
    public required bool IsReversed { get; init; }
    public required decimal Total { get; init; }
    public required IReadOnlyList<JournalLineDto> Lines { get; init; }
}

/// <summary>Every entry of one unit + what the company earned from it.</summary>
public sealed class UnitJournalDto
{
    public required IReadOnlyList<JournalEntryDto> Entries { get; init; }
    /// <summary>Revenue accounts: credit − debit (reversals already netted).</summary>
    public required decimal NetRevenue { get; init; }
}
