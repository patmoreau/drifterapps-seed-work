using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Seeds.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddHttpContextAccessor()
            .AddTransient<IUserContext, UserContext>()
            .AddMemoryCache();

        return services;
    }
}
