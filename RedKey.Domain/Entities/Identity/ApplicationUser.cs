using Microsoft.AspNetCore.Identity;
using RedKey.Domain.Common;

namespace RedKey.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>, IAuditableEntity
{
    public ApplicationUser()
    {
        Id = Guid.CreateVersion7();
        SecurityStamp = Guid.NewGuid().ToString();
    }

    public string FullName { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "ar";
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public Customer? Customer { get; set; }
    public Broker? Broker { get; set; }
}
