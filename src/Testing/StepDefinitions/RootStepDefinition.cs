using DrifterApps.Seeds.Testing.Drivers;
using FluentAssertions;

namespace DrifterApps.Seeds.Testing.StepDefinitions;

public abstract class RootStepDefinition
{
    protected HttpClientDriver HttpClientDriver { get; }

    protected RootStepDefinition(HttpClientDriver httpClientDriver) => HttpClientDriver = httpClientDriver;

    public Guid WithCreatedId()
    {
        var result = HttpClientDriver.DeserializeContent<Created>();

        result.Should().NotBeNull();

        return result!.Id;
    }

    public TResult WithResultAs<TResult>()
    {
        var result = HttpClientDriver.DeserializeContent<TResult>();
        result.Should().NotBeNull();

        return result!;
    }

    private sealed record Created(Guid Id);
}
