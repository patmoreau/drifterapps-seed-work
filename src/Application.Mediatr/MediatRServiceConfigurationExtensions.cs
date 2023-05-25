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
    /// <remarks>It strongly recommended to call this method before anything else in your MediatR configuration</remarks>
    public static MediatRServiceConfiguration RegisterServicesFromApplicationSeeds(
        this MediatRServiceConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .RegisterServicesFromAssemblies(typeof(ApplicationMediatR).Assembly);

        return config;
    }
}
