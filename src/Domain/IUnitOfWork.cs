namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Interface to implement a unit of work pattern
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    ///     Start a unit of work
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>Async <see cref="Task" /></returns>
    Task BeginWorkAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Commit this unit of work
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>Async <see cref="Task" /></returns>
    Task CommitWorkAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Rollback this unit of work
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>Async <see cref="Task" /></returns>
    Task RollbackWorkAsync(CancellationToken cancellationToken);
}
