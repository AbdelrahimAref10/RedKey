using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedKey.Domain.Common;
using RedKey.Domain.Entities.Identity;
using RedKey.Persistence.Contexts;

namespace RedKey.Persistence.Seed;

public static class DatabaseInitializer
{
    /// <summary>
    /// Applies pending migrations (when asked) and seeds the roles + the super admin.
    /// </summary>
    public static async Task InitialiseDatabaseAsync(
        this IServiceProvider services,
        bool applyMigrations,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;
        var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DatabaseInitializer));

        if (applyMigrations)
            await provider.GetRequiredService<RedKeyDbContext>().Database.MigrateAsync(cancellationToken);

        await SeedRolesAsync(provider.GetRequiredService<RoleManager<ApplicationRole>>(), cancellationToken);
        await SeedSuperAdminAsync(
            provider.GetRequiredService<UserManager<ApplicationUser>>(),
            provider.GetRequiredService<IOptions<SuperAdminSeedOptions>>().Value,
            logger);
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, CancellationToken cancellationToken)
    {
        var existing = await roleManager.Roles
            .AsNoTracking()
            .Select(r => r.Name!)
            .ToListAsync(cancellationToken);

        foreach (var role in RoleNames.All.Except(existing))
            await roleManager.CreateAsync(new ApplicationRole(role));
    }

    private static async Task SeedSuperAdminAsync(
        UserManager<ApplicationUser> userManager,
        SuperAdminSeedOptions options,
        ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(options.Email) || string.IsNullOrWhiteSpace(options.Password))
        {
            logger.LogWarning("Super admin seed skipped: '{Section}' is not configured.", SuperAdminSeedOptions.SectionName);
            return;
        }

        if (await userManager.FindByEmailAsync(options.Email) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = options.Email,
            Email = options.Email,
            EmailConfirmed = true,
            PhoneNumber = options.PhoneNumber,
            PhoneNumberConfirmed = options.PhoneNumber is not null,
            FullName = options.FullName
        };

        var result = await userManager.CreateAsync(admin, options.Password);
        if (!result.Succeeded)
        {
            logger.LogError("Super admin seed failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, RoleNames.SuperAdmin);
        logger.LogInformation("Super admin '{Email}' seeded.", options.Email);
    }
}
