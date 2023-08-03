using DrifterApps.Seeds.Application;
using Hangfire;
using MediatR;

namespace DrifterApps.Seeds.Infrastructure;

internal sealed class RequestScheduler : IRequestScheduler
{
    private readonly IBackgroundJobClientFactoryV2 _backgroundJobClientFactoryV2;
    private readonly IRecurringJobManagerFactoryV2 _recurringJobManagerFactoryV2;
    private readonly IRequestExecutor _requestExecutor;

    public RequestScheduler(
        IRequestExecutor requestExecutor,
        IBackgroundJobClientFactoryV2 backgroundJobClientFactoryV2,
        IRecurringJobManagerFactoryV2 recurringJobManagerFactoryV2)
    {
        _requestExecutor = requestExecutor ?? throw new ArgumentNullException(nameof(requestExecutor));
        _backgroundJobClientFactoryV2 = backgroundJobClientFactoryV2;
        _recurringJobManagerFactoryV2 = recurringJobManagerFactoryV2;
    }

    public string SendNow<TBaseRequest>(TBaseRequest request, string description) where TBaseRequest : IBaseRequest
    {
        var mediatorSerializedObject = MediatorSerializedObject.SerializeObject(request, description);

        var job = _backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        return job.Enqueue(() => _requestExecutor.ExecuteCommandAsync(mediatorSerializedObject));
    }

    public void Schedule<TBaseRequest>(TBaseRequest request, DateTimeOffset scheduleAt, string description)
        where TBaseRequest : IBaseRequest
    {
        var mediatorSerializedObject = MediatorSerializedObject.SerializeObject(request, description);

        var job = _backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        job.Schedule(() => _requestExecutor.ExecuteCommandAsync(mediatorSerializedObject), scheduleAt);
    }

    public void Schedule<TBaseRequest>(TBaseRequest request, TimeSpan delay, string description)
        where TBaseRequest : IBaseRequest
    {
        var mediatorSerializedObject = MediatorSerializedObject.SerializeObject(request, description);

        var newTime = DateTime.Now + delay;
        var job = _backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        job.Schedule(() => _requestExecutor.ExecuteCommandAsync(mediatorSerializedObject), newTime);
    }

    public void ScheduleRecurring<TBaseRequest>(TBaseRequest request, string name, string cronExpression,
        string description) where TBaseRequest : IBaseRequest
    {
        var mediatorSerializedObject = MediatorSerializedObject.SerializeObject(request, description);

        var job = _recurringJobManagerFactoryV2.GetManagerV2(JobStorage.Current);
        job.AddOrUpdate(name, () => _requestExecutor.ExecuteCommandAsync(mediatorSerializedObject), cronExpression);
    }
}
