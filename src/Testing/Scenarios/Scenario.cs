using DrifterApps.Seeds.Testing.Drivers;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using Xunit;
using Xunit.Abstractions;

namespace DrifterApps.Seeds.Testing.Scenarios;

/// <summary>
///     Base class for scenario tests in this namespace
///     This class will clear the database using the Respawn package at beginning of every test.
///     Implements Xunit.IAsyncLifetime to manage cleaning up after async tasks.
/// </summary>
public abstract partial class Scenario : IAsyncLifetime
{
    private static readonly AsyncLock Mutex = new();

    protected Scenario(IApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(applicationDriver);
        ArgumentNullException.ThrowIfNull(testOutputHelper);

        ApplicationDriver = applicationDriver;
        TestOutputHelper = testOutputHelper;

        Scope = applicationDriver.Services.CreateScope();

        HttpClientDriver = applicationDriver.CreateHttpClientDriver(testOutputHelper);
    }

    /// <inheritdoc cref="IApplicationDriver" />
    protected IApplicationDriver ApplicationDriver { get; }

    /// <inheritdoc cref="ITestOutputHelper" />
    protected ITestOutputHelper TestOutputHelper { get; }

    /// <inheritdoc cref="IHttpClientDriver" />
    protected IHttpClientDriver HttpClientDriver { get; }

    protected IServiceScope Scope { get; }

    /// <inheritdoc />
    public virtual async Task InitializeAsync()
    {
        // Version for reset every test
        using (await Mutex.LockAsync())
        {
            await ApplicationDriver.ResetStateAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public virtual Task DisposeAsync() => Task.CompletedTask;

    protected async Task ScenarioFor(string description, Action<IScenarioRunner> scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        var runner = ScenarioRunner.Create(description, TestOutputHelper);

        scenario(runner);

        await runner.PlayAsync().ConfigureAwait(false);
    }
}
