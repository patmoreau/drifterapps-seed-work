using System.Text.Json;
using Hangfire;
using MediatR;

namespace DrifterApps.Seeds.Application.Hangfire;

internal sealed class CommandsScheduler : IRequestScheduler
{
    private readonly CommandsExecutor _commandsExecutor;
    private readonly IBackgroundJobClientFactoryV2 _backgroundJobClientFactoryV2;
    private readonly IRecurringJobManagerFactoryV2 _recurringJobManagerFactoryV2;

    public CommandsScheduler(
        CommandsExecutor commandsExecutor,
        IBackgroundJobClientFactoryV2 backgroundJobClientFactoryV2,
        IRecurringJobManagerFactoryV2 recurringJobManagerFactoryV2)
    {
        _commandsExecutor = commandsExecutor;
        _backgroundJobClientFactoryV2 = backgroundJobClientFactoryV2;
        _recurringJobManagerFactoryV2 = recurringJobManagerFactoryV2;
    }

    public string SendNow(IBaseRequest request, string description)
    {
        var mediatorSerializedObject = SerializeObject(request, description);

        var job = _backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        return job.Enqueue(() => _commandsExecutor.ExecuteCommand(mediatorSerializedObject));
    }

    public void Schedule(IBaseRequest request, DateTimeOffset scheduleAt, string description)
    {
        var mediatorSerializedObject = SerializeObject(request, description);

        var job = _backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        job.Schedule(() => _commandsExecutor.ExecuteCommand(mediatorSerializedObject), scheduleAt);
    }

    public void Schedule(IBaseRequest request, TimeSpan delay, string description)
    {
        var mediatorSerializedObject = SerializeObject(request, description);

        var newTime = DateTime.Now + delay;
        var job = _backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        job.Schedule(() => _commandsExecutor.ExecuteCommand(mediatorSerializedObject), newTime);
    }

    public void ScheduleRecurring(IBaseRequest request, string name, string cronExpression, string description)
    {
        var mediatorSerializedObject = SerializeObject(request, description);

        var job = _recurringJobManagerFactoryV2.GetManagerV2(JobStorage.Current);
        job.AddOrUpdate(name, () => _commandsExecutor.ExecuteCommand(mediatorSerializedObject), cronExpression);
    }

    private static MediatorSerializedObject SerializeObject(object mediatorObject, string description)
    {
        var fullTypeName = mediatorObject.GetType().FullName ?? mediatorObject.GetType().Name;
        var data = JsonSerializer.Serialize(mediatorObject);

        return new MediatorSerializedObject(fullTypeName, data, description);
    }
}
