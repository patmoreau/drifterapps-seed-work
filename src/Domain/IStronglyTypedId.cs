namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Represents a strongly-typed identifier interface.
/// </summary>
public interface IStronglyTypedId
{
    /// <summary>
    ///     Gets the GUID value of the strongly-typed identifier.
    /// </summary>
    Guid Value { get; }
}
