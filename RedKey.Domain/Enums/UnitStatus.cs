namespace RedKey.Domain.Enums;

/// <summary>Only <see cref="Approved"/> units are visible on the website.</summary>
public enum UnitStatus : byte
{
    PendingReview = 1,
    Approved = 2,
    ChangesRequested = 3,
    Rejected = 4
}
