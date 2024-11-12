// ReSharper disable once CheckNamespace

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace Xunit.Categories;
#pragma warning restore IDE0130

/// <summary>
///     Attribute to categorize end to end tests
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class EndToEndTestAttribute : CategoryAttribute
{
    private const string Type = "EndToEndTest";

    /// <inheritdoc />
    public EndToEndTestAttribute() : base(Type)
    {
    }
}
