using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Represents a strongly-typed identifier based on a GUID.
/// </summary>
/// <typeparam name="T">The type of the strongly-typed identifier.</typeparam>
[SuppressMessage("Design", "CA1000:Do not declare static members on generic types")]
[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates")]
[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
[SuppressMessage("Major Code Smell", "S4035:Classes implementing \"IEquatable<T>\" should be sealed")]
public abstract record StronglyTypedId<T> : IStronglyTypedId, IEqualityComparer<T>, IComparable<T>, IParsable<T>
    where T : StronglyTypedId<T>, new()
{
    /// <summary>
    ///     Creates a new instance of the strongly-typed identifier with a new GUID value.
    /// </summary>
    public static T New => new() {Value = Guid.NewGuid()};

    /// <summary>
    ///     Creates a new instance of the strongly-typed identifier with an empty GUID value.
    /// </summary>
    public static T Empty => new() {Value = Guid.Empty};

    /// <summary>
    ///     Compares the current instance with another strongly-typed identifier.
    /// </summary>
    /// <param name="other">The strongly-typed identifier to compare with.</param>
    /// <returns>An integer that indicates the relative order of the objects being compared.</returns>
    public int CompareTo(T? other) => other is null ? 1 : Value.CompareTo(other.Value);

    /// <summary>
    ///     Determines whether the specified strongly-typed identifiers are equal.
    /// </summary>
    /// <param name="x">The first strongly-typed identifier to compare.</param>
    /// <param name="y">The second strongly-typed identifier to compare.</param>
    /// <returns>true if the specified strongly-typed identifiers are equal; otherwise, false.</returns>
    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together")]
    public bool Equals(T? x, T? y)
    {
        if (x is null && y is null)
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.Equals(y);
    }

    /// <summary>
    ///     Returns a hash code for the specified strongly-typed identifier.
    /// </summary>
    /// <param name="obj">The strongly-typed identifier.</param>
    /// <returns>A hash code for the specified strongly-typed identifier.</returns>
    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together")]
    public int GetHashCode([DisallowNull] T obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return obj.Value.GetHashCode();
    }

    /// <summary>
    ///     Parses the string representation of a GUID to a strongly-typed identifier.
    /// </summary>
    /// <param name="s">The string representation of the GUID.</param>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <returns>
    ///     A new instance of the strongly-typed identifier if the parse operation succeeds; otherwise, an empty
    ///     identifier.
    /// </returns>
    public static T Parse(string s, IFormatProvider? provider) =>
        Guid.TryParse(s, provider, out var guid) ? Create(guid) : Empty;

    /// <summary>
    ///     Tries to parse the string representation of a GUID to a strongly-typed identifier.
    /// </summary>
    /// <param name="s">The string representation of the GUID.</param>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <param name="result">
    ///     When this method returns, contains the strongly-typed identifier equivalent to the GUID contained
    ///     in <paramref name="s" />, if the parse operation succeeds, or an empty identifier if the parse operation fails.
    ///     This parameter is passed uninitialized.
    /// </param>
    /// <returns><c>true</c> if <paramref name="s" /> was converted successfully; otherwise, <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
        [MaybeNullWhen(false)] out T result)
    {
        if (s is null)
        {
            result = Empty;
            return false;
        }

        if (Guid.TryParse(s, provider, out var guid))
        {
            result = Create(guid);
            return true;
        }

        result = Empty;
        return false;
    }

    /// <summary>
    ///     Gets the GUID value of the strongly-typed identifier.
    /// </summary>
    public Guid Value { get; init; }

    /// <summary>
    ///     Creates a new instance of the strongly-typed identifier with the specified GUID value.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>A new instance of the strongly-typed identifier.</returns>
    public static T Create(Guid value) => new() {Value = value};

    /// <summary>
    ///     Determines whether the current instance is equal to another strongly-typed identifier.
    /// </summary>
    /// <param name="other">The strongly-typed identifier to compare with.</param>
    /// <returns>true if the current instance is equal to the other strongly-typed identifier; otherwise, false.</returns>
    public bool Equals(T? other) => other is not null && Value.Equals(other.Value);

    /// <summary>
    ///     Returns a hash code for the current instance.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    ///     Determines whether one strongly-typed identifier is greater than another.
    /// </summary>
    /// <param name="a">The first strongly-typed identifier.</param>
    /// <param name="b">The second strongly-typed identifier.</param>
    /// <returns>true if the first strongly-typed identifier is greater than the second; otherwise, false.</returns>
    public static bool operator >(StronglyTypedId<T>? a, StronglyTypedId<T>? b) =>
        a switch
        {
            null => false,
            not null when b is null => true,
            _ => a.CompareTo((T?) b) > 0
        };

    /// <summary>
    ///     Determines whether one strongly-typed identifier is less than another.
    /// </summary>
    /// <param name="a">The first strongly-typed identifier.</param>
    /// <param name="b">The second strongly-typed identifier.</param>
    /// <returns>true if the first strongly-typed identifier is less than the second; otherwise, false.</returns>
    public static bool operator <(StronglyTypedId<T>? a, StronglyTypedId<T>? b) =>
        a switch
        {
            null when b is null => false,
            not null when b is null => false,
            null => true,
            _ => a.CompareTo((T?) b) < 0
        };

    /// <summary>
    ///     Determines whether one strongly-typed identifier is greater than or equal to another.
    /// </summary>
    /// <param name="a">The first strongly-typed identifier.</param>
    /// <param name="b">The second strongly-typed identifier.</param>
    /// <returns>true if the first strongly-typed identifier is greater than or equal to the second; otherwise, false.</returns>
    public static bool operator >=(StronglyTypedId<T>? a, StronglyTypedId<T>? b) =>
        a switch
        {
            null when b is null => true,
            null => false,
            not null when b is null => true,
            _ => a.CompareTo((T?) b) >= 0
        };

    /// <summary>
    ///     Determines whether one strongly-typed identifier is less than or equal to another.
    /// </summary>
    /// <param name="a">The first strongly-typed identifier.</param>
    /// <param name="b">The second strongly-typed identifier.</param>
    /// <returns>true if the first strongly-typed identifier is less than or equal to the second; otherwise, false.</returns>
    public static bool operator <=(StronglyTypedId<T>? a, StronglyTypedId<T>? b) =>
        a switch
        {
            null when b is null => true,
            not null when b is null => false,
            null => true,
            _ => a.CompareTo((T?) b) <= 0
        };

    /// <summary>
    ///     Implicitly converts a strongly-typed identifier to a GUID.
    /// </summary>
    /// <param name="stronglyTypedId">The strongly-typed identifier.</param>
#pragma warning disable CA1062
    public static implicit operator Guid(StronglyTypedId<T> stronglyTypedId) => stronglyTypedId!.Value;
#pragma warning restore CA1062

    /// <summary>
    ///     Implicitly converts a GUID to a strongly-typed identifier.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    public static implicit operator StronglyTypedId<T>(Guid value) => new T {Value = value};
}
