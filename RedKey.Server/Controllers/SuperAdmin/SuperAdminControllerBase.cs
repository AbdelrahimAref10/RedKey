using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedKey.Domain.Common;

namespace RedKey.Server.Controllers.SuperAdmin;

/// <summary>Base for every super admin controller: <c>api/admin/{controller}</c>, super admins only.</summary>
[ApiController]
[Authorize(Roles = RoleNames.SuperAdmin)]
[Route("api/admin/[controller]")]
[Produces("application/json")]
public abstract class SuperAdminControllerBase : ControllerBase;
