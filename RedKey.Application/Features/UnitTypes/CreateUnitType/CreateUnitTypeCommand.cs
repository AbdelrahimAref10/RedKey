using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.UnitTypes.CreateUnitType;

public sealed record CreateUnitTypeCommand : IRequest<Result<int>>
{
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public int SortOrder { get; init; }

    /// <summary>Icon shown in the landing circle (base64 / data-URL, png or svg). Optional.</summary>
    public string? Icon { get; init; }
}
