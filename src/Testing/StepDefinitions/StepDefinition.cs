using DrifterApps.Seeds.Testing.Drivers;
using FluentAssertions;

namespace DrifterApps.Seeds.Testing.StepDefinitions;

public abstract class StepDefinition
{
    protected StepDefinition(IHttpClientDriver httpClientDriver) => HttpClientDriver = httpClientDriver;
    protected IHttpClientDriver HttpClientDriver { get; }

    public async Task<Guid> WithCreatedIdAsync()
    {
        var result = await HttpClientDriver.DeserializeContentAsync<Created>().ConfigureAwait(false);

        result.Should().NotBeNull();

        return result!.Id;
    }

    public async Task<TResult> WithResultAs<TResult>()
    {
        var result = await HttpClientDriver.DeserializeContentAsync<TResult>().ConfigureAwait(false);
        result.Should().NotBeNull();

        return result!;
    }

    internal sealed record Created(Guid Id);
}
