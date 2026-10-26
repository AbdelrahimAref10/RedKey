using RedKey.Application.Dtos.Units;
using RedKey.Domain.Enums;
using RedKey.Domain.ValueObjects;

namespace RedKey.Application.Features.Units.Common;

/// <summary>The unit form (create + update), filled by the super admin or a broker.</summary>
public abstract record UnitInput
{
    public required int UnitTypeId { get; init; }
    public required int AreaId { get; init; }
    /// <summary>Value from the lookups endpoint (sale / rent / investment).</summary>
    public required int Purpose { get; init; }
    public required string TitleAr { get; init; }
    public required string TitleEn { get; init; }
    public required string CompoundAr { get; init; }
    public required string CompoundEn { get; init; }
    public required decimal Price { get; init; }
    public required int Bedrooms { get; init; }
    public required int Bathrooms { get; init; }
    public required decimal Size { get; init; }
    /// <summary>Value from the lookups endpoint.</summary>
    public required int Finishing { get; init; }
    public bool IsFeatured { get; init; }
    public decimal BrokerCommission { get; init; }
    /// <summary>Super admin only (ignored when a broker sends it): paid by the customer when the viewing takes place. 0 = free.</summary>
    public decimal? ViewingFee { get; init; }
    public required string DescriptionAr { get; init; }
    public required string DescriptionEn { get; init; }
    public string? DeliveryAr { get; init; }
    public string? DeliveryEn { get; init; }

    /// <summary>Card image: a base64 / data-URL for a new image, or the existing URL to keep it.</summary>
    public required string CoverImage { get; init; }

    /// <summary>Up to 3 detail-page images (base64 for new ones, existing URL to keep).</summary>
    public IReadOnlyList<string> GalleryImages { get; init; } = [];

    public IReadOnlyList<PaymentPlanInput> PaymentPlans { get; init; } = [];
    public IReadOnlyList<FeatureInput> Features { get; init; } = [];

    /// <param name="cityId">The city of the selected area (from <see cref="UnitLookupGuard"/>).</param>
    internal UnitContent ToContent(int cityId) => new(
        UnitTypeId, cityId, AreaId, (UnitPurpose)Purpose,
        TitleAr.Trim(), TitleEn.Trim(), CompoundAr.Trim(), CompoundEn.Trim(),
        Price, Bedrooms, Bathrooms, Size, (FinishingType)Finishing,
        IsFeatured, BrokerCommission,
        DescriptionAr.Trim(), DescriptionEn.Trim(),
        string.IsNullOrWhiteSpace(DeliveryAr) ? null : DeliveryAr.Trim(),
        string.IsNullOrWhiteSpace(DeliveryEn) ? null : DeliveryEn.Trim());

    internal IEnumerable<PaymentPlanContent> ToPaymentPlans() =>
        PaymentPlans.Select(p => new PaymentPlanContent(p.LabelAr.Trim(), p.LabelEn.Trim(), p.ValueAr.Trim(), p.ValueEn.Trim()));

    internal IEnumerable<LocalizedPair> ToFeatures() =>
        Features.Select(f => new LocalizedPair(f.NameAr.Trim(), f.NameEn.Trim()));
}
