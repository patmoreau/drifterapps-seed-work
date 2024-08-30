namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Interface to identify the root of an aggregate
///     <seealso href="https://martinfowler.com/bliki/DDD_Aggregate.html">Martin Fowler</seealso>
/// </summary>
public interface IAggregateRoot<out T> : IAggregateRoot where T : IStronglyTypedId
{
    /// <summary>
    ///     Aggregate Id
    /// </summary>
    public T Id { get; }
}
