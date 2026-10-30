using RedKey.Application.Dtos.Units;

namespace RedKey.Application.Dtos.Favorites;

/// <summary>One saved unit: who saved it (name, phone, email), the unit itself and what happened after.</summary>
public sealed class FavoriteAdminDto
{
    public required Guid CustomerUserId { get; init; }
    public required string CustomerName { get; init; }
    public string? CustomerPhone { get; init; }
    public string? CustomerEmail { get; init; }

    /// <summary>Full unit card data (admin view: status + creator included).</summary>
    public required UnitListItemDto Unit { get; init; }

    public required DateTime SavedAtUtc { get; init; }

    /// <summary>The same customer also asked to view this unit (latest request status, translated).</summary>
    public required bool RequestedViewing { get; init; }
    public string? ViewingStatusName { get; init; }
}

/// <summary>The most saved units: how many customers saved each one and how many of them asked for a viewing.</summary>
public sealed class FavoriteUnitStatsDto
{
    public required UnitListItemDto Unit { get; init; }
    public required int SavedCount { get; init; }
    public required int RequestedViewingCount { get; init; }
    public required DateTime LastSavedAtUtc { get; init; }
}
