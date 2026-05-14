namespace DrifterApps.Seeds.Testing.Attributes;

/// <summary>
///     Attribute to categorize component tests
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class UnitTestAttribute : CategoryAttribute
{
    private const string Type = "ComponentTest";

    /// <inheritdoc />
    public UnitTestAttribute() : base(Type)
    {
    }
}
