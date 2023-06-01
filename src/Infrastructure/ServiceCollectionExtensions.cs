using DrifterApps.Seeds.Application;
using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Seeds.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Add Hangfire Request Scheduler support for MediatR
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    /// <returns>
    ///     <see cref="IServiceCollection" />
    /// </returns>
    public static IServiceCollection AddHangfireRequestScheduler(this IServiceCollection services)
    {
        services
            .AddTransient<IRequestScheduler, RequestScheduler>()
            .AddTransient<IRequestExecutor, RequestExecutor>();

        return services;
    }
}
