using System.ComponentModel;
using MediatR;

namespace DrifterApps.Seeds.Infrastructure;

internal sealed class RequestExecutor(ISender mediator) : IRequestExecutor
{
    [DisplayName("Processing request {0}")]
    public async Task ExecuteCommandAsync(MediatorSerializedObject mediatorSerializedObject)
    {
        if (mediatorSerializedObject.TryDeserializeObject(out var request) && request is not null)
            await mediator.Send(request!).ConfigureAwait(false);
    }
}
