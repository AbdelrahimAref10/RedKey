using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Dtos.Auth;
using RedKey.Application.Features.Account.UpdateMyLanguage;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.General;

[ApiController]
[Authorize]
[Route("api/account")]
[Produces("application/json")]
public sealed class AccountController : ControllerBase
{
    private readonly ISender _sender;

    public AccountController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Saves the session language (admin / broker) and returns a fresh token with the new <c>lang</c> claim.</summary>
    [HttpPut("language")]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateLanguage([FromBody] UpdateMyLanguageCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command, cancellationToken)).ToActionResult();
}
