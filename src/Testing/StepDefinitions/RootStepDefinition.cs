using DrifterApps.Seeds.Tests.Drivers;
using FluentAssertions;

namespace DrifterApps.Seeds.Tests.StepDefinitions;

public abstract class RootStepDefinition
{
    protected HttpClientDriver HttpClientDriver { get; }

    protected RootStepDefinition(HttpClientDriver httpClientDriver) => HttpClientDriver = httpClientDriver;

    public Guid WithCreatedId()
    {
        Created? result = HttpClientDriver.DeserializeContent<Created>();

        result.Should().NotBeNull();

        return result!.Id;
    }

    public TResult WithResultAs<TResult>()
    {
        TResult? result = HttpClientDriver.DeserializeContent<TResult>();
        result.Should().NotBeNull();

        return result!;
    }

    private sealed record Created(Guid Id);
}
