using DrifterApps.Seeds.Application;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Seeds.Infrastructure;

internal sealed class RequestScheduler(
    IRequestExecutor requestExecutor,
    IBackgroundJobClientFactoryV2 backgroundJobClientFactoryV2,
    IRecurringJobManagerFactoryV2 recurringJobManagerFactoryV2,
    IJsonSerializerOptionsFactory jsonSerializerOptionsFactory,
    ILogger<RequestScheduler> logger)
    : IRequestScheduler
{
    private readonly IRequestExecutor _requestExecutor =
        requestExecutor ?? throw new ArgumentNullException(nameof(requestExecutor));

    public string SendNow<TBaseRequest>(TBaseRequest request, string description) where TBaseRequest : IBaseRequest
    {
        logger.LogInformation("{RequestExecutor}: {Description}", nameof(SendNow), description);

        var mediatorSerializedObject = CreateMediatorSerializedObject(request, description);

        var job = backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        return job.Enqueue(() => _requestExecutor.ExecuteCommandAsync(mediatorSerializedObject));
    }

    public void Schedule<TBaseRequest>(TBaseRequest request, DateTimeOffset scheduleAt, string description)
        where TBaseRequest : IBaseRequest
    {
        logger.LogInformation("{RequestExecutor} on {ScheduleAt}: {Description}", nameof(Schedule), scheduleAt,
            description);

        var mediatorSerializedObject = CreateMediatorSerializedObject(request, description);

        var job = backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        job.Schedule(() => _requestExecutor.ExecuteCommandAsync(mediatorSerializedObject), scheduleAt);
    }

    public void Schedule<TBaseRequest>(TBaseRequest request, TimeSpan delay, string description)
        where TBaseRequest : IBaseRequest
    {
        logger.LogInformation("{RequestExecutor} in {Delay}: {Description}", nameof(Schedule), delay, description);

        var mediatorSerializedObject = CreateMediatorSerializedObject(request, description);

        var newTime = DateTime.Now + delay;
        var job = backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        job.Schedule(() => _requestExecutor.ExecuteCommandAsync(mediatorSerializedObject), newTime);
    }

    public void ScheduleRecurring<TBaseRequest>(TBaseRequest request, string name, string cronExpression,
        string description) where TBaseRequest : IBaseRequest
    {
        logger.LogInformation("{RequestExecutor}: {Description}", nameof(ScheduleRecurring), description);

        var mediatorSerializedObject = CreateMediatorSerializedObject(request, description);

        var job = recurringJobManagerFactoryV2.GetManagerV2(JobStorage.Current);
        job.AddOrUpdate(name, () => _requestExecutor.ExecuteCommandAsync(mediatorSerializedObject), cronExpression);
    }

    private MediatorSerializedObject CreateMediatorSerializedObject<TBaseRequest>(TBaseRequest request,
        string description)
        where TBaseRequest : IBaseRequest
    {
        var options = jsonSerializerOptionsFactory.CreateOptions();
        var mediatorSerializedObject = MediatorSerializedObject.SerializeObject(request, description, options);

        logger.LogDebug("{@MediatorSerializedObject}", mediatorSerializedObject);

        return mediatorSerializedObject;
    }
}
