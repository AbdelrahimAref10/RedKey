namespace RedKey.Application.Dtos.Lookups;

public sealed class LookupItemDto
{
    public required int Value { get; init; }
    public required string Name { get; init; }
}

/// <summary>All back-end enums, translated (feeds every dropdown in the front-end).</summary>
public sealed class LookupsDto
{
    public required IReadOnlyList<LookupItemDto> UnitPurposes { get; init; }
    public required IReadOnlyList<LookupItemDto> FinishingTypes { get; init; }
    public required IReadOnlyList<LookupItemDto> PreferredTimes { get; init; }
    public required IReadOnlyList<LookupItemDto> UnitStatuses { get; init; }
    public required IReadOnlyList<LookupItemDto> ReviewDecisions { get; init; }
    public required IReadOnlyList<LookupItemDto> ViewingRequestStatuses { get; init; }
    public required IReadOnlyList<LookupItemDto> ContactSubjects { get; init; }
}

public sealed class UnitTypeDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? IconUrl { get; init; }
}

public sealed class CityDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}

public sealed class AreaDto
{
    public required int Id { get; init; }
    public required int CityId { get; init; }
    public required string Name { get; init; }
}

public sealed class UnitTypeAdminDto
{
    public required int Id { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public string? IconUrl { get; init; }
    public required bool IsActive { get; init; }
    public required int SortOrder { get; init; }
}

public sealed class CityAdminDto
{
    public required int Id { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public required bool IsActive { get; init; }
    public required int SortOrder { get; init; }
    public required int AreasCount { get; init; }
}

public sealed class AreaAdminDto
{
    public required int Id { get; init; }
    public required int CityId { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public required bool IsActive { get; init; }
    public required int SortOrder { get; init; }
}

/// <summary>What deleting a city / area / unit type did: removed, or only deactivated because units use it.</summary>
public sealed class LookupDeleteResultDto
{
    public required bool Deleted { get; init; }
}
