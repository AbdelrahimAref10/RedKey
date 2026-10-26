namespace RedKey.Domain.Entities.Accounting;

/// <summary>One side of a journal entry: a debit or a credit on one account.</summary>
public class JournalLine
{
    private JournalLine() { }

    public long Id { get; private set; }
    public Guid EntryId { get; private set; }
    public int AccountId { get; private set; }
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
    public string? Memo { get; private set; }

    public Account Account { get; private set; } = null!;

    internal static JournalLine Create(int accountId, decimal debit, decimal credit, string? memo) => new()
    {
        AccountId = accountId,
        Debit = debit,
        Credit = credit,
        Memo = memo
    };
}
