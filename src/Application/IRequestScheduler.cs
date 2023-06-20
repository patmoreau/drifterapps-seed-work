using MediatR;

namespace DrifterApps.Seeds.Application;

/// <summary>
///     Interface for a request schedule.
/// </summary>
public interface IRequestScheduler
{
    /// <summary>
    ///     Send the request now.
    /// </summary>
    /// <param name="request">
    ///     <see cref="IBaseRequest" />
    /// </param>
    /// <param name="description">task description</param>
    /// <returns></returns>
    string SendNow(IBaseRequest request, string description);

    /// <summary>
    ///     Schedule the request.
    /// </summary>
    /// <param name="request">
    ///     <see cref="IBaseRequest" />
    /// </param>
    /// <param name="scheduleAt"><see cref="DateTimeOffset" /> to be scheduled at</param>
    /// <param name="description">task description</param>
    void Schedule(IBaseRequest request, DateTimeOffset scheduleAt, string description);

    /// <summary>
    ///     Delay the request.
    /// </summary>
    /// <param name="request">
    ///     <see cref="IBaseRequest" />
    /// </param>
    /// <param name="delay"><see cref="TimeSpan" /> delay before starting the request</param>
    /// <param name="description">task description</param>
    void Schedule(IBaseRequest request, TimeSpan delay, string description);

    /// <summary>
    ///     Schedule a recurring request.
    /// </summary>
    /// <param name="request">
    ///     <see cref="IBaseRequest" />
    /// </param>
    /// <param name="name">name of the request</param>
    /// <param name="cronExpression">cron expression</param>
    /// <param name="description">task description</param>
    void ScheduleRecurring(IBaseRequest request, string name, string cronExpression, string description);
}
