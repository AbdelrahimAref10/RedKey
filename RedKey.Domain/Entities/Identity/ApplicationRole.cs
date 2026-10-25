using Microsoft.AspNetCore.Identity;

namespace RedKey.Domain.Entities.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole()
    {
        Id = Guid.CreateVersion7();
    }

    public ApplicationRole(string roleName) : this()
    {
        Name = roleName;
        NormalizedName = roleName.ToUpperInvariant();
    }
}
