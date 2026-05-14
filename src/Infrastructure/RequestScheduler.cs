using System.Linq.Expressions;
using DrifterApps.Seeds.Application;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Seeds.Infrastructure;

internal sealed partial class RequestScheduler(
    IBackgroundJobClientFactoryV2 backgroundJobClientFactoryV2,
    ILogger<RequestScheduler> logger)
    : IRequestScheduler
{
    public string QueueHandler<THandler>(Expression<Func<THandler, Task>> methodCall, string description)
    {
        LogRequestexecutorDescription(nameof(QueueHandler), description);
        var job = backgroundJobClientFactoryV2.GetClientV2(JobStorage.Current);
        return job.Enqueue(methodCall);
    }

    [LoggerMessage(LogLevel.Information, "{RequestExecutor}: {Description}")]
    partial void LogRequestexecutorDescription(string requestExecutor, string description);
}
