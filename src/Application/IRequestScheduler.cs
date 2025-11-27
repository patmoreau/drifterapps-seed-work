using System.Linq.Expressions;

namespace DrifterApps.Seeds.Application;

/// <summary>
///     Interface for a request schedule.
/// </summary>
public interface IRequestScheduler
{
    /// <summary>
    ///     Queue handler call.
    /// </summary>
    /// <param name="methodCall">
    /// </param>
    /// <param name="description">task description</param>
    /// <returns></returns>
    string QueueHandler<THandler>(Expression<Func<THandler, Task>> methodCall, string description);
}
