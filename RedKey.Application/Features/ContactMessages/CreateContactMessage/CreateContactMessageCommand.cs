using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.ContactMessages.CreateContactMessage;

/// <summary>A signed-in customer sends a message from the website contact form.</summary>
public sealed record CreateContactMessageCommand : IRequest<Result<Guid>>
{
    public required string Name { get; init; }

    /// <summary>ISO 3166 alpha-2 of the picked country, e.g. EG.</summary>
    public required string CountryIso { get; init; }

    /// <summary>Country calling code, e.g. 20 (a leading "+" is ignored).</summary>
    public required string DialCode { get; init; }

    /// <summary>National number; spaces and a leading 0 are removed before saving.</summary>
    public required string PhoneNumber { get; init; }

    /// <summary>Value from the lookups endpoint (contact subjects).</summary>
    public required int Subject { get; init; }

    public required string Message { get; init; }
}
