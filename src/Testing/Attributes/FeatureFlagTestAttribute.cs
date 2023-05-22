using Xunit.Abstractions;
using Xunit.Sdk;

// ReSharper disable once CheckNamespace
namespace Xunit.Categories;

/// <summary>
///     Attribute to categorize feature flag tests
/// </summary>
/// <example>
///     [FeatureFlagTest(FeatureFlags.Temporary.MyNewFeature)]
/// </example>
[TraitDiscoverer(FeatureFlagTestDiscoverer.DiscovererTypeName, "Common")]
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
}

internal sealed class FeatureFlagTestDiscoverer : ITraitDiscoverer
{
    internal const string DiscovererTypeName = "Xunit.Categories.FeatureFlagTestDiscoverer";

    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)

    {
        var name = traitAttribute.GetNamedArgument<string>("Identifier");

        yield return new KeyValuePair<string, string>("Category", FeatureFlagTestAttribute.Type);

        if (!string.IsNullOrWhiteSpace(name))
            yield return new KeyValuePair<string, string>(FeatureFlagTestAttribute.Type, name);
    }
}
