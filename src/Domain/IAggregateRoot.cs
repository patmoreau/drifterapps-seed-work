using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Interface to identify the root of an aggregate
///     <seealso href="https://martinfowler.com/bliki/DDD_Aggregate.html">Martin Fowler</seealso>
/// </summary>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces")]
public interface IAggregateRoot;
