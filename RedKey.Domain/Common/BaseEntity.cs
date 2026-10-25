namespace RedKey.Domain.Common;

public abstract class BaseEntity : IAuditableEntity
{
    public Guid Id { get; protected set; } = Guid.CreateVersion7();

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
