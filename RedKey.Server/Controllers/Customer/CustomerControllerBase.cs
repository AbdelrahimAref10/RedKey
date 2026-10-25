using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedKey.Domain.Common;

namespace RedKey.Server.Controllers.Customer;

/// <summary>Base for every customer controller: <c>api/customer/{controller}</c>, customers only.</summary>
[ApiController]
[Authorize(Roles = RoleNames.Customer)]
[Route("api/customer/[controller]")]
[Produces("application/json")]
public abstract class CustomerControllerBase : ControllerBase;
