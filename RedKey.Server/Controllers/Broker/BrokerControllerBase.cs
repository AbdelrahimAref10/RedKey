using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedKey.Domain.Common;

namespace RedKey.Server.Controllers.Broker;

/// <summary>Base for every broker controller: <c>api/broker/{controller}</c>, brokers only.</summary>
[ApiController]
[Authorize(Roles = RoleNames.Broker)]
[Route("api/broker/[controller]")]
[Produces("application/json")]
public abstract class BrokerControllerBase : ControllerBase;
