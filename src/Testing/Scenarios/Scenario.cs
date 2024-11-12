using DrifterApps.Seeds.Testing.Drivers;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using Refit;
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
    protected const string ContextHttpResponse = $"{nameof(Scenario)}_HttpResponse";

    private static readonly AsyncLock Mutex = new();
    private ScenarioRunner? _runner;

    protected Scenario(IApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(applicationDriver);
        ArgumentNullException.ThrowIfNull(testOutputHelper);

        ApplicationDriver = applicationDriver;
        TestOutputHelper = testOutputHelper;

        Scope = applicationDriver.Services.CreateScope();
    }

    /// <inheritdoc cref="IApplicationDriver" />
    protected IApplicationDriver ApplicationDriver { get; }

    /// <inheritdoc cref="ITestOutputHelper" />
    protected ITestOutputHelper TestOutputHelper { get; }

    protected IServiceScope Scope { get; }

    /// <inheritdoc />
    public virtual async Task InitializeAsync()
    {
        // Version for reset every test
        using (await Mutex.LockAsync())
        {
            await ResetStateAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public virtual Task DisposeAsync() => Task.CompletedTask;

    protected abstract Task ResetStateAsync(CancellationToken cancellationToken);

    protected async Task ScenarioFor(string description, Action<IScenarioRunner> scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        _runner = ScenarioRunner.Create(description, TestOutputHelper);

        scenario(_runner);

        await _runner.PlayAsync().ConfigureAwait(false);
    }

    protected async Task<TApiResponse> HttpCall<TApiResponse>(Func<Task<TApiResponse>> httpCall)
        where TApiResponse : IApiResponse
    {
        ArgumentNullException.ThrowIfNull(httpCall);

        if (_runner is null)
        {
            throw new InvalidOperationException("ScenarioRunner is not initialized.");
        }

        var response = await httpCall().ConfigureAwait(false);

        _runner.SetContextData(ContextHttpResponse, response);

        return response;
    }
}
