using RedKey.Application.Common.Localization;
using RedKey.Application.Dtos.Units;
using RedKey.Domain.Entities.Units;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Units.Common;

/// <summary>Flat row read from SQL in the requested language (enum names are added in memory).</summary>
internal sealed record UnitRow(
    Guid Id,
    string Code,
    string Title,
    string Compound,
    int CityId,
    string CityName,
    int AreaId,
    string AreaName,
    int UnitTypeId,
    string UnitTypeName,
    UnitPurpose Purpose,
    decimal Price,
    int Bedrooms,
    int Bathrooms,
    decimal Size,
    FinishingType Finishing,
    string CoverImageUrl,
    bool IsFeatured,
    decimal BrokerCommission,
    decimal ViewingFee,
    UnitStatus Status,
    string? ReviewNote,
    Guid CreatedByUserId,
    UserRole CreatedByRole,
    string CreatedByName,
    DateTime CreatedAtUtc);

internal static class UnitProjection
{
    /// <summary>Visibility rules of each scope.</summary>
    public static IQueryable<PropertyUnit> ForScope(this IQueryable<PropertyUnit> units, UnitsScope scope, Guid? userId) =>
        scope switch
        {
            UnitsScope.Public or UnitsScope.BrokerCatalog => units.Where(u => u.Status == UnitStatus.Approved),
            UnitsScope.BrokerOwn => units.Where(u => u.CreatedByUserId == userId),
            UnitsScope.Admin => units,
            _ => units.Where(_ => false)
        };

    /// <summary>Single SQL projection; the language switch becomes a CASE on a parameter.</summary>
    public static IQueryable<UnitRow> ToRows(this IQueryable<PropertyUnit> units, bool ar) =>
        units.Select(u => new UnitRow(
            u.Id,
            u.Code,
            ar ? u.TitleAr : u.TitleEn,
            ar ? u.CompoundAr : u.CompoundEn,
            u.CityId,
            ar ? u.City.NameAr : u.City.NameEn,
            u.AreaId,
            ar ? u.Area.NameAr : u.Area.NameEn,
            u.UnitTypeId,
            ar ? u.UnitType.NameAr : u.UnitType.NameEn,
            u.Purpose,
            u.Price,
            u.Bedrooms,
            u.Bathrooms,
            u.Size,
            u.Finishing,
            u.CoverImageUrl,
            u.IsFeatured,
            u.BrokerCommission,
            u.ViewingFee,
            u.Status,
            u.ReviewNote,
            u.CreatedByUserId,
            u.CreatedByRole,
            u.CreatedBy.FullName,
            u.CreatedAtUtc));

    public static UnitListItemDto ToListItem(this UnitRow r, UnitsScope scope, bool ar) =>
        new UnitListItemDto
        {
            Id = r.Id, Code = r.Code, Title = r.Title, Compound = r.Compound,
            CityId = r.CityId, CityName = r.CityName, AreaId = r.AreaId, AreaName = r.AreaName, UnitTypeId = r.UnitTypeId, UnitTypeName = r.UnitTypeName,
            Purpose = (int)r.Purpose, PurposeName = EnumNames.Of(r.Purpose, ar),
            Price = r.Price, Bedrooms = r.Bedrooms, Bathrooms = r.Bathrooms, Size = r.Size,
            Finishing = (int)r.Finishing, FinishingName = EnumNames.Of(r.Finishing, ar),
            CoverImageUrl = r.CoverImageUrl, IsFeatured = r.IsFeatured, CreatedAtUtc = r.CreatedAtUtc,
            BrokerCommission = scope == UnitsScope.Public ? null : r.BrokerCommission,
            ViewingFee = r.ViewingFee,
            Status = ShowsStatus(scope) ? (int)r.Status : null,
            StatusName = ShowsStatus(scope) ? EnumNames.Of(r.Status, ar) : null,
            ReviewNote = ShowsStatus(scope) ? r.ReviewNote : null,
            CreatedBy = scope == UnitsScope.Admin ? Creator(r, ar) : null
        };

    public static UnitCreatorDto Creator(UnitRow r, bool ar) => new()
    {
        UserId = r.CreatedByUserId,
        FullName = r.CreatedByName,
        Role = (int)r.CreatedByRole,
        RoleName = EnumNames.Of(r.CreatedByRole, ar)
    };

    private static bool ShowsStatus(UnitsScope scope) => scope is UnitsScope.BrokerOwn or UnitsScope.Admin;
}
