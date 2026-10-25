using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Dtos.Auth;
using RedKey.Application.Features.Auth.Login;
using RedKey.Application.Features.Auth.Register;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.General;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Login with email or phone number. Works for super admin, broker and customer.</summary>
    [HttpPost("login")]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status423Locked)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command, cancellationToken)).ToActionResult();

    /// <summary>Register a broker (role = 1) or a customer (role = 2).</summary>
    [HttpPost("register")]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command, cancellationToken)).ToActionResult();
}
