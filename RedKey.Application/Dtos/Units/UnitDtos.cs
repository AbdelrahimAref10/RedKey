namespace RedKey.Application.Dtos.Units;

// Enum fields are plain numbers + a translated name: the front-end never declares the enums itself.

public class UnitListItemDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Title { get; init; }
    public required string Compound { get; init; }
    public required int CityId { get; init; }
    public required string CityName { get; init; }
    public required int AreaId { get; init; }
    public required string AreaName { get; init; }
    public required int UnitTypeId { get; init; }
    public required string UnitTypeName { get; init; }
    public required int Purpose { get; init; }
    public required string PurposeName { get; init; }
    public required decimal Price { get; init; }
    public required int Bedrooms { get; init; }
    public required int Bathrooms { get; init; }
    public required decimal Size { get; init; }
    public required int Finishing { get; init; }
    public required string FinishingName { get; init; }
    public required string CoverImageUrl { get; init; }
    public required bool IsFeatured { get; init; }

    /// <summary>Brokers and admins only (null on the public website).</summary>
    public decimal? BrokerCommission { get; init; }

    /// <summary>What the customer pays when the viewing takes place (0 = free).</summary>
    public required decimal ViewingFee { get; init; }

    /// <summary>Brokers (own units) and admins only.</summary>
    public int? Status { get; init; }
    public string? StatusName { get; init; }
    public string? ReviewNote { get; init; }

    /// <summary>Admins only: who added the unit.</summary>
    public UnitCreatorDto? CreatedBy { get; init; }

    public required DateTime CreatedAtUtc { get; init; }
}

public sealed class UnitCreatorDto
{
    public required Guid UserId { get; init; }
    public required string FullName { get; init; }
    public required int Role { get; init; }
    public required string RoleName { get; init; }
}

public sealed class UnitDetailsDto
{
    public required UnitListItemDto Unit { get; init; }
    public required string Description { get; init; }
    public string? Delivery { get; init; }
    public required IReadOnlyList<string> Images { get; init; }
    public required IReadOnlyList<PaymentPlanDto> PaymentPlans { get; init; }
    public required IReadOnlyList<string> Features { get; init; }
}

public sealed class PaymentPlanDto
{
    public required string Label { get; init; }
    public required string Value { get; init; }
}

/// <summary>Both languages, for the admin / broker edit form.</summary>
public sealed class UnitEditDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required int UnitTypeId { get; init; }
    public required int CityId { get; init; }
    public required int AreaId { get; init; }
    public required int Purpose { get; init; }
    public required string TitleAr { get; init; }
    public required string TitleEn { get; init; }
    public required string CompoundAr { get; init; }
    public required string CompoundEn { get; init; }
    public required decimal Price { get; init; }
    public required int Bedrooms { get; init; }
    public required int Bathrooms { get; init; }
    public required decimal Size { get; init; }
    public required int Finishing { get; init; }
    public required bool IsFeatured { get; init; }
    public required decimal BrokerCommission { get; init; }
    public required decimal ViewingFee { get; init; }
    public required string DescriptionAr { get; init; }
    public required string DescriptionEn { get; init; }
    public string? DeliveryAr { get; init; }
    public string? DeliveryEn { get; init; }
    public required string CoverImageUrl { get; init; }
    public required IReadOnlyList<string> GalleryImages { get; init; }
    public required IReadOnlyList<PaymentPlanInput> PaymentPlans { get; init; }
    public required IReadOnlyList<FeatureInput> Features { get; init; }
    public required int Status { get; init; }
    public required string StatusName { get; init; }
    public string? ReviewNote { get; init; }
    public UnitCreatorDto? CreatedBy { get; init; }
}

public sealed class PaymentPlanInput
{
    public required string LabelAr { get; init; }
    public required string LabelEn { get; init; }
    public required string ValueAr { get; init; }
    public required string ValueEn { get; init; }
}

public sealed class FeatureInput
{
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
}
