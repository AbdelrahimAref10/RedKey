using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedKey.Application.Contracts.Persistence;
using RedKey.Persistence.Contexts;
using RedKey.Persistence.Interceptors;
using RedKey.Persistence.Seed;

namespace RedKey.Persistence.DependencyInjection;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<RedKeyDbContext>((provider, options) =>
            options
                .UseSqlServer(connectionString, sql =>
                    sql.MigrationsAssembly(typeof(RedKeyDbContext).Assembly.FullName))
                .AddInterceptors(provider.GetRequiredService<AuditableEntityInterceptor>()));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<RedKeyDbContext>());

        services.Configure<SuperAdminSeedOptions>(configuration.GetSection(SuperAdminSeedOptions.SectionName));

        return services;
    }
}
