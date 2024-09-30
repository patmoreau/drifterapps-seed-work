namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Represents a primitive type with a value of type <typeparamref name="T" />.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public interface IPrimitiveType<out T> where T : notnull
{
    /// <summary>
    ///     Gets the value of the primitive type.
    /// </summary>
    T Value { get; }
}
