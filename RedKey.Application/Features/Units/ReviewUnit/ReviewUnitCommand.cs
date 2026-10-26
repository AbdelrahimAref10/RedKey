using System.Text.Json.Serialization;
using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.Units.ReviewUnit;

/// <summary>Super admin decision on a unit (approve / request changes / reject) + an optional note for the broker.</summary>
public sealed record ReviewUnitCommand : IRequest<Result>
{
    [JsonIgnore]
    public Guid Id { get; init; }

    /// <summary>Value from the lookups endpoint (review decisions).</summary>
    public required int Decision { get; init; }

    public string? Note { get; init; }
}
