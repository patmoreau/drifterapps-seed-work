using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Seeds.Application.Hangfire;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryHangfireServices(this IServiceCollection services)
    {
        // Add Hangfire services.
        services.AddHangfire(globalConfiguration => globalConfiguration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMemoryStorage());

        // Add the processing server as IHostedService
        services.AddHangfireServer();

        services.AddScoped<IRequestScheduler, CommandsScheduler>()
            .AddScoped<CommandsExecutor>();

        return services;
    }
}
