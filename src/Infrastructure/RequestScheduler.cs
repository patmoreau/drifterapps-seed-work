using DrifterApps.Seeds.Application;
using Hangfire;
using MediatR;

namespace DrifterApps.Seeds.Infrastructure;

internal sealed class RequestScheduler : IRequestScheduler
{
    private readonly IBackgroundJobClientFactoryV2 _backgroundJobClientFactoryV2;
    private readonly IRecurringJobManagerFactoryV2 _recurringJobManagerFactoryV2;
    private readonly RequestExecutor _requestExecutor;

    public RequestScheduler(
        RequestExecutor requestExecutor,
        IBackgroundJobClientFactoryV2 backgroundJobClientFactoryV2,
        IRecurringJobManagerFactoryV2 recurringJobManagerFactoryV2)
    {
        _requestExecutor = requestExecutor;
        _backgroundJobClientFactoryV2 = backgroundJobClientFactoryV2;
        _recurringJobManagerFactoryV2 = recurringJobManagerFactoryV2;
    }

    public string SendNow(IBaseRequest request, string description)
    {
        var mediatorSerializedObject = MediatorSerializedObject.SerializeObject(request, description);

        var job = _backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        return job.Enqueue(() => _requestExecutor.ExecuteCommand(mediatorSerializedObject));
    }

    public void Schedule(IBaseRequest request, DateTimeOffset scheduleAt, string description)
    {
        var mediatorSerializedObject = MediatorSerializedObject.SerializeObject(request, description);

        var job = _backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        job.Schedule(() => _requestExecutor.ExecuteCommand(mediatorSerializedObject), scheduleAt);
    }

    public void Schedule(IBaseRequest request, TimeSpan delay, string description)
    {
        var mediatorSerializedObject = MediatorSerializedObject.SerializeObject(request, description);

        var newTime = DateTime.Now + delay;
        var job = _backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        job.Schedule(() => _requestExecutor.ExecuteCommand(mediatorSerializedObject), newTime);
    }

    public void ScheduleRecurring(IBaseRequest request, string name, string cronExpression, string description)
    {
        var mediatorSerializedObject = MediatorSerializedObject.SerializeObject(request, description);

        var job = _recurringJobManagerFactoryV2.GetManagerV2(JobStorage.Current);
        job.AddOrUpdate(name, () => _requestExecutor.ExecuteCommand(mediatorSerializedObject), cronExpression);
    }
}
