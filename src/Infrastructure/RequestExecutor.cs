using System.ComponentModel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Seeds.Infrastructure;

internal sealed class RequestExecutor(ISender mediator, ILogger<RequestExecutor> logger) : IRequestExecutor
{
    [DisplayName("Processing request {0}")]
    public async Task ExecuteCommandAsync(MediatorSerializedObject mediatorSerializedObject)
    {
        logger.LogDebug("{RequestExecutor}: {@MediatorSerializedObject}", nameof(ExecuteCommandAsync),
            mediatorSerializedObject);
        if (mediatorSerializedObject.TryDeserializeObject(out var request) && request is not null)
        {
            logger.LogInformation("{RequestExecutor}: {Request}", nameof(ExecuteCommandAsync), request.ToString());
            await mediator.Send(request!).ConfigureAwait(false);
        }
        else
        {
            logger.LogError("Unable to deserialize object {@MediatorSerializedObject}", mediatorSerializedObject);
        }
    }
}
