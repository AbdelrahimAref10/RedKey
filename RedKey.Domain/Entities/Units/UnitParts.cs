using RedKey.Domain.ValueObjects;

namespace RedKey.Domain.Entities.Units;

/// <summary>Detail-page data (1:1 with <see cref="PropertyUnit"/>, keyed by UnitId).</summary>
public class UnitDetails
{
    internal UnitDetails() { }

    public Guid UnitId { get; private set; }
    public string DescriptionAr { get; private set; } = string.Empty;
    public string DescriptionEn { get; private set; } = string.Empty;
    public string? DeliveryAr { get; private set; }
    public string? DeliveryEn { get; private set; }

    internal void Set(string descriptionAr, string descriptionEn, string? deliveryAr, string? deliveryEn)
    {
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        DeliveryAr = deliveryAr;
        DeliveryEn = deliveryEn;
    }
}

/// <summary>One of the (up to 3) gallery images on the detail page.</summary>
public class UnitImage
{
    private UnitImage() { }

    public int Id { get; private set; }
    public Guid UnitId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }

    internal static UnitImage Create(string url, int sortOrder) => new() { Url = url, SortOrder = sortOrder };
}

public class UnitPaymentPlan
{
    private UnitPaymentPlan() { }

    public int Id { get; private set; }
    public Guid UnitId { get; private set; }
    public string LabelAr { get; private set; } = string.Empty;
    public string LabelEn { get; private set; } = string.Empty;
    public string ValueAr { get; private set; } = string.Empty;
    public string ValueEn { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }

    internal static UnitPaymentPlan Create(PaymentPlanContent p, int sortOrder) => new()
    {
        LabelAr = p.LabelAr,
        LabelEn = p.LabelEn,
        ValueAr = p.ValueAr,
        ValueEn = p.ValueEn,
        SortOrder = sortOrder
    };
}

public class UnitFeature
{
    private UnitFeature() { }

    public int Id { get; private set; }
    public Guid UnitId { get; private set; }
    public string NameAr { get; private set; } = string.Empty;
    public string NameEn { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }

    internal static UnitFeature Create(LocalizedPair name, int sortOrder) => new()
    {
        NameAr = name.Ar,
        NameEn = name.En,
        SortOrder = sortOrder
    };
}
