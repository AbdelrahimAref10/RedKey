using RedKey.Domain.Common;

namespace RedKey.Domain.Entities;

/// <summary>A unit type (apartment, villa, compound...) managed by the super admin.</summary>
public class UnitType : IAuditableEntity
{
    private UnitType() { }

    public int Id { get; private set; }
    public string NameAr { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    /// <summary>Icon shown inside the circle on the landing page.</summary>
    public string? IconUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; }
    /// <summary>Soft delete: hidden everywhere, the row stays for the deleted units that still reference it.</summary>
    public bool IsDeleted { get; private set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public static UnitType Create(string nameAr, string nameEn, int sortOrder) => new()
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

    public void SetIcon(string? iconUrl) => IconUrl = iconUrl;

    public void Deactivate() => IsActive = false;

    public void Delete() => IsDeleted = true;
}
