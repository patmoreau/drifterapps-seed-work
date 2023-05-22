using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Seeds.Application.Mediatr;

/// <summary>
///     <see cref="MediatRServiceConfiguration" /> extensions
/// </summary>
public static class MediatRServiceConfigurationExtensions
{
    /// <summary>
    ///     Register MediatR services from DrifterApps.Seeds.Application.MediatR
    /// </summary>
    /// <param name="config">
    ///     <see cref="MediatRServiceConfiguration" />
    /// </param>
    /// <returns>
    ///     <see cref="MediatRServiceConfiguration" />
    /// </returns>
    public static MediatRServiceConfiguration RegisterServicesFromApplication(this MediatRServiceConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config.RegisterServicesFromAssemblies(typeof(ApplicationMediatR).Assembly)
            .AddOpenBehavior(typeof(LoggingBehavior<,>));

        return config;
    }
}
