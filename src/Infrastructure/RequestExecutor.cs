using System.ComponentModel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Seeds.Infrastructure;

internal sealed class RequestExecutor(
    IJsonSerializerOptionsFactory jsonSerializerOptionsFactory,
    ISender mediator,
    ILogger<RequestExecutor> logger) : IRequestExecutor
{
    [DisplayName("Processing request {0}")]
    public async Task ExecuteCommandAsync(MediatorSerializedObject mediatorSerializedObject)
    {
        logger.LogDebug("{RequestExecutor}: {@MediatorSerializedObject}", nameof(ExecuteCommandAsync),
            mediatorSerializedObject);
        var options = jsonSerializerOptionsFactory.CreateOptions();
        if (mediatorSerializedObject.TryDeserializeObject(out var request, options) && request is not null)
        {
            logger.LogInformation("{RequestExecutor}: {Request}", nameof(ExecuteCommandAsync), request.ToString());
            await mediator.Send(request).ConfigureAwait(false);
        }
        else
        {
            logger.LogError("Unable to deserialize object {@MediatorSerializedObject}", mediatorSerializedObject);
        }
    }
}
