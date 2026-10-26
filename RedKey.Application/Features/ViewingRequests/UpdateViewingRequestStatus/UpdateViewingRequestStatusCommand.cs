using System.Text.Json.Serialization;
using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.ViewingRequests.UpdateViewingRequestStatus;

/// <summary>
/// The super admin moves a viewing request on; the customer is notified.
/// "Viewing done" posts its fee to the accounts, leaving "viewing done" reverses that entry.
/// </summary>
public sealed record UpdateViewingRequestStatusCommand : IRequest<Result>
{
    [JsonIgnore]
    public Guid Id { get; init; }

    /// <summary>Value from the lookups endpoint (viewing request statuses).</summary>
    public required int Status { get; init; }

    /// <summary>
    /// "Viewing done" with a fee: the cash / bank account that received it (from the cash accounts endpoint).
    /// Empty = the cash box.
    /// </summary>
    public int? CashAccountId { get; init; }
}
