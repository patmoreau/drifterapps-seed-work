using Xunit.v3;

namespace DrifterApps.Seeds.Testing.Attributes;

/// <summary>
///     Attribute to categorize feature flag tests
/// </summary>
/// <example>
///     [FeatureFlagTest(FeatureFlags.Temporary.MyNewFeature)]
/// </example>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class FeatureFlagTestAttribute : Attribute, ITraitAttribute
{
    internal const string Type = "FeatureFlagTest";

    /// <inheritdoc />
    public FeatureFlagTestAttribute(string identifier)
    {
        ArgumentException.ThrowIfNullOrEmpty(identifier);

        Identifier = identifier;
    }

    /// <summary>
    ///     Feature flag identifier
    /// </summary>
    public string Identifier { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() =>
    [
        new("Category", Type),
        new(Type, Identifier)
    ];
}
