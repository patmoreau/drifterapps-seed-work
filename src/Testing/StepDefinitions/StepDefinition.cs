using DrifterApps.Seeds.Testing.Drivers;
using FluentAssertions;

namespace DrifterApps.Seeds.Testing.StepDefinitions;

public abstract class StepDefinition(IHttpClientDriver httpClientDriver)
{
    protected IHttpClientDriver HttpClientDriver { get; } = httpClientDriver;

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

    internal sealed record Created(Guid Id);
}
