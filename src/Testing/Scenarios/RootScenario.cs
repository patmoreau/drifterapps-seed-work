using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using DrifterApps.Seeds.Tests.Drivers;
using DrifterApps.Seeds.Tests.Infrastructure;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using Xunit;
using Xunit.Abstractions;

namespace DrifterApps.Seeds.Tests.Scenarios;

/// <summary>
///     <P>Base class for scenario tests in this namespace</P>
///     <P>This class will clear the database using the Respawn package at beginning of every test.</P>
///     Implements Xunit.IAsyncLifetime to manage cleaning up after async tasks.
/// </summary>
public abstract partial class RootScenario : IAsyncLifetime
{
    private static readonly AsyncLock s_mutex = new();

    private readonly IApplicationDriver _applicationDriver;
    private readonly ITestOutputHelper _testOutputHelper;

    protected RootScenario(IApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(applicationDriver);

        _applicationDriver = applicationDriver;
        _testOutputHelper = testOutputHelper;

        HttpClientDriver = applicationDriver.CreateHttpClientDriver(testOutputHelper);
    }

    protected HttpClientDriver HttpClientDriver { get; }

    public virtual async Task InitializeAsync()
    {
        // Version for reset every test
        using (await s_mutex.LockAsync())
        {
            await _applicationDriver.OnScenarioReset().ConfigureAwait(false);
        }
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;

    internal Task WhenUserTriesToQuery(ApiResource apiResources, int? offset = null, int? limit = null,
        string? sorts = null, string? filters = null)
    {
        StringBuilder sb = new();
        if (offset is not null)
        {
            sb.Append(CultureInfo.InvariantCulture, $"offset={offset}&");
        }

        if (limit is not null)
        {
            sb.Append(CultureInfo.InvariantCulture, $"limit={limit}&");
        }

        if (!string.IsNullOrWhiteSpace(sorts))
        {
            foreach (string sort in sorts.Split(';'))
            {
                sb.Append(CultureInfo.InvariantCulture, $"sort={sort}&");
            }
        }

        if (!string.IsNullOrWhiteSpace(filters))
        {
            foreach (string filter in filters.Split(';'))
            {
                sb.Append(CultureInfo.InvariantCulture, $"filter={filter}&");
            }
        }

        return HttpClientDriver.SendGetRequest(apiResources,
            sb.Length == 0 ? null : sb.Remove(sb.Length - 1, 1).ToString());
    }

    protected void ThenShouldNotHaveInternalServerError() =>
        HttpClientDriver.ShouldHaveResponseWithStatus(statusCode => statusCode != HttpStatusCode.InternalServerError);

    protected void ThenUserShouldBeAuthorizedToAccessEndpoint() => CheckAuthorizationStatus(true);

    protected void ShouldBeForbiddenToAccessEndpoint() => CheckAuthorizationStatus(false);

    protected void ShouldNotBeAuthorizedToAccessEndpoint() => CheckAuthorizationStatus(false);

    protected TContent ThenShouldReceive<TContent>()
    {
        var result = HttpClientDriver.DeserializeContent<TContent>();
        result.Should().NotBeNull();
        return result!;
    }

    protected void ThenShouldExpectStatusCode(HttpStatusCode expectedStatusCode) =>
        HttpClientDriver.ShouldHaveResponseWithStatus(expectedStatusCode);

    protected void ThenShouldReceiveProblemDetailsWithErrorMessage(HttpStatusCode expectedStatusCode,
        string errorMessage)
    {
        ThenShouldExpectStatusCode(expectedStatusCode);

        var problemDetails = HttpClientDriver.DeserializeContent<ProblemDetails>();
        problemDetails.Should()
            .NotBeNull()
            .And.BeAssignableTo<ProblemDetails>()
            .Subject.Detail.Should().Be(errorMessage);
    }

    protected void ShouldReceiveValidationProblemDetailsWithErrorMessage(string errorMessage)
    {
        ThenShouldExpectStatusCode(HttpStatusCode.UnprocessableEntity);

        var problemDetails = HttpClientDriver.DeserializeContent<ValidationProblemDetails>();
        problemDetails.Should()
            .NotBeNull()
            .And.BeAssignableTo<ValidationProblemDetails>()
            .Subject.Title.Should()
            .Be(errorMessage);
    }

    protected Guid ThenShouldGetTheRouteOfTheNewResourceInTheHeader()
    {
        var headers = HttpClientDriver.ResponseMessage!.Headers;

        headers.Should().ContainKey("Location");

        var responseString = headers.GetValues("Location").Single();
        var match = MyRegex().Match(responseString);

        return match.Success ? Guid.Parse(match.Value) : Guid.Empty;
    }

#pragma warning disable CA1822
    protected void ThenAssertAll(Func<Task> assertions)
#pragma warning restore CA1822
    {
        ArgumentNullException.ThrowIfNull(assertions);

        using AssertionScope scope = new();
        assertions();
    }

#pragma warning disable CA1822
    protected void ThenAssertAll(Action assertions)
#pragma warning restore CA1822
    {
        ArgumentNullException.ThrowIfNull(assertions);

        using AssertionScope scope = new();
        assertions();
    }

    private void CheckAuthorizationStatus(bool isAuthorized)
    {
        bool IsExpectedStatus(HttpStatusCode? statusCode)
        {
            return isAuthorized
                ? statusCode is not (HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)
                : statusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized;
        }

        HttpClientDriver.ShouldHaveResponseWithStatus(IsExpectedStatus);
    }

    protected async Task ScenarioFor(string description, Action<IScenarioRunner> scenario)
    {
        ArgumentNullException.ThrowIfNull(scenario);

        ScenarioRunner runner = ScenarioRunner.Create(description, _testOutputHelper);

        scenario(runner);

        await runner.PlayAsync().ConfigureAwait(false);
    }

    [GeneratedRegex("[{(]?[0-9A-Fa-f]{8}[-]?([0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?")]
    private static partial Regex MyRegex();
}
