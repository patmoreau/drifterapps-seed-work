using Xunit.v3;

namespace DrifterApps.Seeds.Testing.Attributes;

/// <summary>
///     Base attribute for categorizing tests with a single category trait.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public abstract class CategoryAttribute : Attribute, ITraitAttribute
{
    private readonly string _category;

    /// <inheritdoc cref="CategoryAttribute" />
    protected CategoryAttribute(string category)
    {
        ArgumentException.ThrowIfNullOrEmpty(category);
        _category = category;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() =>
        [new("Category", _category)];
}
