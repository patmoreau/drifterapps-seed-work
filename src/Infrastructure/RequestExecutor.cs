using System.ComponentModel;
using MediatR;

namespace DrifterApps.Seeds.Infrastructure;

internal sealed class RequestExecutor
{
    private readonly IMediator _mediator;

    public RequestExecutor(IMediator mediator) => _mediator = mediator;

    [DisplayName("Processing request {0}")]
    public async Task ExecuteCommand(MediatorSerializedObject mediatorSerializedObject)
    {
        if (mediatorSerializedObject.TryDeserializeObject(out var request))
            await _mediator.Send(request!).ConfigureAwait(false);
    }
}
