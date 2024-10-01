using System.Text.Json;
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
    /// <param name="jsonOptions"></param>
    /// <returns>
    ///     <see cref="IServiceCollection" />
    /// </returns>
    public static IServiceCollection AddHangfireRequestScheduler(this IServiceCollection services,
        Func<JsonSerializerOptions>? jsonOptions = null)
    {
        services
            .AddTransient<IRequestScheduler, RequestScheduler>()
            .AddTransient<IRequestExecutor, RequestExecutor>()
            .AddSingleton<IJsonSerializerOptionsFactory>(new JsonSerializerOptionsFactory(jsonOptions));

        return services;
    }
}
