using System.ComponentModel;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using MediatR;

namespace DrifterApps.Seeds.Application.Hangfire;

internal sealed class CommandsExecutor
{
    private readonly IMediator _mediator;

    public CommandsExecutor(IMediator mediator) => _mediator = mediator;

    [DisplayName("Processing command {0}")]
    public async Task ExecuteCommand(MediatorSerializedObject mediatorSerializedObject)
    {
        var type = Assembly.GetAssembly(typeof(MediaTypeNames.Application))!.GetType(mediatorSerializedObject.FullTypeName);

        if (type != null)
        {
            var req = JsonSerializer.Deserialize(mediatorSerializedObject.Data, type);

            await _mediator.Send((req as IBaseRequest)!).ConfigureAwait(false);
        }
    }
}
