// ReSharper disable once CheckNamespace

namespace Xunit.Categories;

/// <summary>
///     Attribute to categorize component tests
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class ComponentTestAttribute : CategoryAttribute
{
    private const string Type = "ComponentTest";

    /// <inheritdoc />
    public ComponentTestAttribute() : base(Type)
    {
    }
}
