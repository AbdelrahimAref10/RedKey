using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Features.ContactMessages.CreateContactMessage;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.Customer;

/// <summary>The website "send us a message" form (web + mobile); it lands in the super admin inbox live.</summary>
[Route("api/customer/contact-messages")]
public sealed class CustomerContactMessagesController : CustomerControllerBase
{
    private readonly ISender _sender;

    public CustomerContactMessagesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Send([FromBody] CreateContactMessageCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command, cancellationToken)).ToActionResult();
}
