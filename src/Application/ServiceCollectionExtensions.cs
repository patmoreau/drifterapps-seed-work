using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Seeds.Application;

/// <summary>
///     Extensions to <see cref="IServiceCollection" />
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Add <see cref="IUserContext" /> transient implementation
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    /// <returns>
    ///     <see cref="IServiceCollection" />
    /// </returns>
    public static IServiceCollection AddUserContext(this IServiceCollection services)
    {
        services.AddHttpContextAccessor()
            .AddTransient<IUserContext, UserContext>()
            .AddMemoryCache();

        return services;
    }
}
