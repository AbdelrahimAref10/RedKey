using RedKey.Domain.Common;

namespace RedKey.Domain.Entities;

/// <summary>An area inside a city (managed by the super admin).</summary>
public class Area : IAuditableEntity
{
    private Area() { }

    public int Id { get; private set; }
    public int CityId { get; private set; }
    public string NameAr { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; }
    /// <summary>Soft delete: hidden everywhere, the row stays for the deleted units that still reference it.</summary>
    public bool IsDeleted { get; private set; }

    public City City { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public static Area Create(int cityId, string nameAr, string nameEn, int sortOrder) => new()
    {
        CityId = cityId,
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
