using System.ComponentModel;
using MediatR;

namespace DrifterApps.Seeds.Infrastructure;

internal sealed class RequestExecutor : IRequestExecutor
{
    private readonly IMediator _mediator;

    public RequestExecutor(IMediator mediator) => _mediator = mediator;

    [DisplayName("Processing request {0}")]
    public async Task ExecuteCommandAsync(MediatorSerializedObject mediatorSerializedObject)
    {
        if (mediatorSerializedObject.TryDeserializeObject(out var request) && request is not null)
            await _mediator.Send(request!).ConfigureAwait(false);
    }
}
