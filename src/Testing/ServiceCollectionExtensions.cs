using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Logging;

namespace DrifterApps.Seeds.Testing;

public static class ServiceCollectionExtensions
{
    public static AuthenticationBuilder AddMockAuthentication(this IServiceCollection services,
        Action<MockAuthenticationSchemeOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        IdentityModelEventSource.ShowPII = true;
        return services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>()
            .AddAuthentication(MockAuthenticationHandler.AuthenticationScheme)
            .AddScheme<MockAuthenticationSchemeOptions, MockAuthenticationHandler>(
                MockAuthenticationHandler.AuthenticationScheme, configureOptions);
    }

    public static IServiceCollection AddDatabaseDriver<TDbContext>(this IServiceCollection services,
        DatabaseDriver<TDbContext> databaseDriver) where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(databaseDriver);

        services.RemoveAll<TDbContext>();
        services.RemoveAll<DbContextOptions>();

        if (services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TDbContext>)) is
            { } dbContextDescriptor)
            services.Remove(dbContextDescriptor);


        services.AddSingleton<DbContextOptions<TDbContext>>(_ => databaseDriver.GetDbContextOptions());
        services.AddSingleton<DbContextOptions>(_ => databaseDriver.GetDbContextOptions());

        services.AddDbContext<TDbContext>();
        services.AddDbContextFactory<TDbContext>();

        return services;
    }
}
