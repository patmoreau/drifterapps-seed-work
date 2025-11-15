using Microsoft.FeatureManagement;

namespace DrifterApps.Seeds.Testing.Drivers;

/// <inheritdoc cref="IFeatureManagerDriver" />
public class FeatureManagerDriver : IFeatureManagerDriver, IFeatureManager
{
    private readonly Dictionary<string, bool> _featureFlags = [];

    /// <inheritdoc />
    public async IAsyncEnumerable<string> GetFeatureNamesAsync()
    {
        foreach (var key in _featureFlags.Keys)
        {
            yield return key;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<bool> IsEnabledAsync(string feature) =>
        Task.FromResult(_featureFlags.TryGetValue(feature, out var value) && value);

    /// <inheritdoc />
    public Task<bool> IsEnabledAsync<TContext>(string feature, TContext context) =>
        Task.FromResult(_featureFlags.TryGetValue(feature, out var value) && value);

    /// <inheritdoc />
    public void Enable(string feature) => _featureFlags[feature] = true;

    /// <inheritdoc />
    public void Disable(string feature) => _featureFlags[feature] = false;
}
