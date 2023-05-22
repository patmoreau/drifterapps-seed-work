namespace DrifterApps.Seeds.Testing.Drivers;

/// <summary>
/// Feature Manager Driver allowing to set feature flags during tests.
/// </summary>
public interface IFeatureManagerDriver
{
    /// <summary>
    /// Enable a feature flag
    /// </summary>
    /// <param name="feature">Name of the feature flag</param>
    void Enable(string feature);

    /// <summary>
    /// Disable a feature flag
    /// </summary>
    /// <param name="feature">Name of the feature flag</param>
    void Disable(string feature);
}
