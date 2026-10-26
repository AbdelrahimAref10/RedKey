using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RedKey.Server.Controllers.Landing;

/// <summary>
/// Base for the public website (visitors, no login). Every endpoint takes <c>?lang=ar|en</c>.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/landing/[controller]")]
[Produces("application/json")]
public abstract class LandingControllerBase : ControllerBase;
