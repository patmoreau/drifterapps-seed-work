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
    string SendNow<TBaseRequest>(TBaseRequest request, string description) where TBaseRequest : IBaseRequest;

    /// <summary>
    ///     Schedule the request.
    /// </summary>
    /// <param name="request">
    ///     <see cref="IBaseRequest" />
    /// </param>
    /// <param name="scheduleAt"><see cref="DateTimeOffset" /> to be scheduled at</param>
    /// <param name="description">task description</param>
    void Schedule<TBaseRequest>(TBaseRequest request, DateTimeOffset scheduleAt, string description)
        where TBaseRequest : IBaseRequest;

    /// <summary>
    ///     Delay the request.
    /// </summary>
    /// <param name="request">
    ///     <see cref="IBaseRequest" />
    /// </param>
    /// <param name="delay"><see cref="TimeSpan" /> delay before starting the request</param>
    /// <param name="description">task description</param>
    void Schedule<TBaseRequest>(TBaseRequest request, TimeSpan delay, string description)
        where TBaseRequest : IBaseRequest;

    /// <summary>
    ///     Schedule a recurring request.
    /// </summary>
    /// <param name="request">
    ///     <see cref="IBaseRequest" />
    /// </param>
    /// <param name="name">name of the request</param>
    /// <param name="cronExpression">cron expression</param>
    /// <param name="description">task description</param>
    void ScheduleRecurring<TBaseRequest>(TBaseRequest request, string name, string cronExpression, string description)
        where TBaseRequest : IBaseRequest;
}
