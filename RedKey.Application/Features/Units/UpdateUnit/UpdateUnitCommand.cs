using System.Text.Json.Serialization;
using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Features.Units.Common;

namespace RedKey.Application.Features.Units.UpdateUnit;

/// <summary>Same command for the super admin and brokers (a broker edit goes back to review).</summary>
public sealed record UpdateUnitCommand : UnitInput, IRequest<Result>
{
    /// <summary>Taken from the route.</summary>
    [JsonIgnore]
    public Guid Id { get; init; }
}
