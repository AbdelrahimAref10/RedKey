using System.Text.Json.Serialization;
using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.ViewingRequests.CreateViewingRequest;

/// <summary>A signed-in customer asks to view a published unit.</summary>
public sealed record CreateViewingRequestCommand : IRequest<Result<Guid>>
{
    [JsonIgnore]
    public Guid UnitId { get; init; }

    public required string Name { get; init; }
    public required string PhoneNumber { get; init; }

    /// <summary>Value from the lookups endpoint (preferred times).</summary>
    public required int PreferredTime { get; init; }
}
