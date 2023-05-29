using DrifterApps.Seeds.Application;
using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Seeds.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireRequestScheduler(this IServiceCollection services)
    {
        services.AddScoped<IRequestScheduler, RequestScheduler>()
            .AddScoped<RequestExecutor>();

        return services;
    }
}
