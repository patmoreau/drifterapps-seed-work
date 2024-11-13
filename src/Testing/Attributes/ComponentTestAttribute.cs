// ReSharper disable once CheckNamespace

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace Xunit.Categories;
#pragma warning restore IDE0130

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
