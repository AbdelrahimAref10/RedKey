using System.Text.Json.Serialization;
using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.UnitTypes.UpdateUnitType;

public sealed record UpdateUnitTypeCommand : IRequest<Result>
{
    [JsonIgnore]
    public int Id { get; init; }

    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; } = true;

    /// <summary>New icon (base64), the current URL to keep it, or empty to remove it.</summary>
    public string? Icon { get; init; }
}
