namespace RedKey.Domain.Enums;

/// <summary>Account class: decides on which side (debit / credit) the account grows.</summary>
public enum AccountType : byte
{
    Asset = 1,
    Liability = 2,
    Equity = 3,
    Revenue = 4,
    Expense = 5
}

public static class AccountTypeExtensions
{
    /// <summary>
    /// Assets and expenses grow on the debit side (balance = debit − credit);
    /// liabilities, equity and revenues grow on the credit side (balance = credit − debit).
    /// </summary>
    public static bool IsDebitNormal(this AccountType type) => type is AccountType.Asset or AccountType.Expense;

    /// <summary>Balance of an account of this type from its debit / credit totals (positive = normal side).</summary>
    public static decimal Balance(this AccountType type, decimal debit, decimal credit) =>
        type.IsDebitNormal() ? debit - credit : credit - debit;
}
