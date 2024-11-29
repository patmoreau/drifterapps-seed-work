using DrifterApps.Seeds.Testing.Drivers;
using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Seeds.Testing.StepDefinitions;

public abstract class ApiSteps<TApi>(IApplicationDriver applicationDriver)
    where TApi : notnull
{
    private readonly Lazy<TApi> _api = new(applicationDriver.Services.GetRequiredService<TApi>);

    protected TApi Api => _api.Value;
}
