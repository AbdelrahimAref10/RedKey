namespace RedKey.Domain.Enums;

public enum ViewingRequestStatus : byte
{
    New = 1,
    Contacted = 2,
    Closed = 3,
    /// <summary>The viewing took place (its fee is posted to the company accounts).</summary>
    Completed = 4
}
