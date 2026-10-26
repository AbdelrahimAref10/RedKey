namespace RedKey.Application.Dtos.ViewingRequests;

/// <summary>A viewing request in the super admin requests table.</summary>
public sealed class ViewingRequestAdminDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string PhoneNumber { get; init; }
    public required int PreferredTime { get; init; }
    public required string PreferredTimeName { get; init; }
    public required int Status { get; init; }
    public required string StatusName { get; init; }
    public required Guid UnitId { get; init; }
    public required string UnitCode { get; init; }
    public required string UnitTitle { get; init; }
    public required string AreaName { get; init; }
    public required string CityName { get; init; }
    /// <summary>Agreed when the request was sent (0 = free).</summary>
    public required decimal ViewingFee { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}

/// <summary>Counts per status (tabs, the sidebar badge and the dashboard).</summary>
public sealed class ViewingRequestsSummaryDto
{
    public required int Total { get; init; }
    public required int New { get; init; }
    public required int Contacted { get; init; }
    public required int Closed { get; init; }
    public required int Completed { get; init; }
}
