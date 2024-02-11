namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Repository interface for saving an aggregate
/// </summary>
/// <typeparam name="TAggregate">aggregate of type <see cref="IAggregateRoot" /></typeparam>
public interface IRepository<in TAggregate> where TAggregate : IAggregateRoot
{
    /// <summary>
    ///     Method to save the aggregate
    /// </summary>
    /// <param name="aggregate">aggregate of type <typeparamref name="TAggregate" /> to save</param>
    /// <param name="cancellationToken">cancellation token<seealso cref="CancellationToken" /></param>
    /// <returns>executable <see cref="Task" /></returns>
    Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken);
}
