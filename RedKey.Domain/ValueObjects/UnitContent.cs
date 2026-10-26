using RedKey.Domain.Enums;

namespace RedKey.Domain.ValueObjects;

/// <summary>Everything the admin / broker types in the unit form (both languages).</summary>
public sealed record UnitContent(
    int UnitTypeId,
    int CityId,
    int AreaId,
    UnitPurpose Purpose,
    string TitleAr,
    string TitleEn,
    string CompoundAr,
    string CompoundEn,
    decimal Price,
    int Bedrooms,
    int Bathrooms,
    decimal Size,
    FinishingType Finishing,
    bool IsFeatured,
    decimal BrokerCommission,
    string DescriptionAr,
    string DescriptionEn,
    string? DeliveryAr,
    string? DeliveryEn);

public sealed record LocalizedPair(string Ar, string En);

public sealed record PaymentPlanContent(string LabelAr, string LabelEn, string ValueAr, string ValueEn);
