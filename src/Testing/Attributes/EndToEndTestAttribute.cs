namespace DrifterApps.Seeds.Testing.Attributes;

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
