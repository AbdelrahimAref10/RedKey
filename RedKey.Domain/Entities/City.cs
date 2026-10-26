using RedKey.Domain.Common;

namespace RedKey.Domain.Entities;

/// <summary>A city the company works in; its areas are listed under it (managed by the super admin).</summary>
public class City : IAuditableEntity
{
    private City() { }

    public int Id { get; private set; }
    public string NameAr { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; }
    /// <summary>Soft delete: hidden everywhere, the row stays for the deleted units that still reference it.</summary>
    public bool IsDeleted { get; private set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public static City Create(string nameAr, string nameEn, int sortOrder) => new()
    {
        NameAr = nameAr,
        NameEn = nameEn,
        SortOrder = sortOrder
    };

    public void Update(string nameAr, string nameEn, int sortOrder, bool isActive)
    {
        NameAr = nameAr;
        NameEn = nameEn;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public void Deactivate() => IsActive = false;

    public void Delete() => IsDeleted = true;
}
