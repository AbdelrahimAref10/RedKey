using RedKey.Domain.Enums;

namespace RedKey.Domain.Entities.Accounting;

/// <summary>An account of the chart of accounts (seeded; system accounts cannot be removed).</summary>
public class Account
{
    private Account() { }

    public int Id { get; private set; }
    /// <summary>Chart code, e.g. 1100 (see <see cref="SystemAccounts"/>).</summary>
    public string Code { get; private set; } = string.Empty;
    public string NameAr { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public AccountType Type { get; private set; }
    /// <summary>Cash / bank: shown in the treasury and allowed as the receiving account of a payment.</summary>
    public bool IsCash { get; private set; }
    public bool IsSystem { get; private set; }
    public bool IsActive { get; private set; } = true;
}

/// <summary>Codes of the seeded accounts the posting rules use.</summary>
public static class SystemAccounts
{
    public const string Cash = "1100";
    public const string Bank = "1200";
    public const string CustomerReceivables = "1300";
    public const string BrokerCommissionsPayable = "2100";
    public const string ViewingFeeRevenue = "4100";
    public const string SaleCommissionRevenue = "4200";
    public const string RentCommissionRevenue = "4300";
    public const string BrokerCommissionExpense = "5100";
}
