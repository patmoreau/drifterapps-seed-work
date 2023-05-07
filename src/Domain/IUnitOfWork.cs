namespace DrifterApps.Seeds.Domain;

/// <summary>
///
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task BeginWorkAsync(CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CommitWorkAsync(CancellationToken cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RollbackWorkAsync(CancellationToken cancellationToken);
}
