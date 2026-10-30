using RedKey.Domain.Common;
using RedKey.Domain.Entities.Identity;
using RedKey.Domain.Entities.Units;

namespace RedKey.Domain.Entities;

/// <summary>A unit a customer saved (web + mobile share it). Key: customer + unit.</summary>
public class Favorite : IAuditableEntity
{
    private Favorite() { }

    public Guid CustomerUserId { get; private set; }
    public Guid UnitId { get; private set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public ApplicationUser Customer { get; private set; } = null!;
    public PropertyUnit Unit { get; private set; } = null!;

    public static Favorite Create(Guid customerUserId, Guid unitId) => new()
    {
        CustomerUserId = customerUserId,
        UnitId = unitId
    };
}
