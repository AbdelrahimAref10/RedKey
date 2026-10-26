namespace RedKey.Domain.Enums;

/// <summary>The business operation a journal entry records (one posting rule each).</summary>
public enum JournalSourceType : byte
{
    /// <summary>A viewing was done: the fee was collected (cash / bank ← viewing fee revenue).</summary>
    ViewingFee = 1
}
