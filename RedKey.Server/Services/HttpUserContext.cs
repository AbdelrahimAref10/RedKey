using RedKey.Application.Contracts.Identity;
using RedKey.Application.Contracts.Localization;
using RedKey.Domain.Common;
using RedKey.Domain.Enums;

namespace RedKey.Server.Services;

/// <summary>Signed-in user taken from the JWT of the current request.</summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private System.Security.Claims.ClaimsPrincipal? User => _accessor.HttpContext?.User;

    public Guid? UserId => Guid.TryParse(User?.FindFirst("sub")?.Value, out var id) ? id : null;
    public UserRole? Role => RoleNames.ToRole(User?.FindFirst("role")?.Value);
    public string? FullName => User?.FindFirst("name")?.Value;
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;
}

/// <summary>
/// <c>?lang=</c> (landing + customer clients, web and mobile) wins; otherwise the session language
/// of the signed-in admin / broker (<c>lang</c> claim); otherwise Arabic.
/// </summary>
public sealed class RequestLanguageContext : ILanguageContext
{
    private readonly IHttpContextAccessor _accessor;

    public RequestLanguageContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string Lang
    {
        get
        {
            var context = _accessor.HttpContext;
            var requested = context?.Request.Query["lang"].ToString();
            if (requested is "ar" or "en")
                return requested;

            return context?.User.FindFirst("lang")?.Value == "en" ? "en" : "ar";
        }
    }

    public bool IsArabic => Lang == "ar";
}
