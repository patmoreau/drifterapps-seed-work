using DrifterApps.Seeds.Testing.Drivers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DrifterApps.Seeds.Testing;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseDriver<TDbContext>(this IServiceCollection services,
        DatabaseDriver<TDbContext> databaseDriver) where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(databaseDriver);

        services.RemoveAll<TDbContext>();
        services.RemoveAll<DbContextOptions>();

        if (services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TDbContext>)) is
            { } dbContextDescriptor)
        {
            services.Remove(dbContextDescriptor);
        }


        services.AddSingleton<DbContextOptions<TDbContext>>(_ => databaseDriver.GetDbContextOptions());
        services.AddSingleton<DbContextOptions>(_ => databaseDriver.GetDbContextOptions());

        services.AddDbContext<TDbContext>();
        services.AddDbContextFactory<TDbContext>();

        return services;
    }
}
