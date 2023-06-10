using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using DrifterApps.Seeds.Testing.Attributes;
using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Infrastructure;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using Xunit;
using Xunit.Abstractions;

namespace DrifterApps.Seeds.Testing.Scenarios;

/// <summary>
///     <P>Base class for scenario tests in this namespace</P>
///     <P>This class will clear the database using the Respawn package at beginning of every test.</P>
///     Implements Xunit.IAsyncLifetime to manage cleaning up after async tasks.
/// </summary>
public abstract partial class RootScenario : IAsyncLifetime
{
    protected const string CreatedResourceLocation = $"{nameof(RootScenario)}-created-resource-location";

    private static readonly AsyncLock Mutex = new();

    private readonly IApplicationDriver _applicationDriver;
    private readonly ITestOutputHelper _testOutputHelper;

    protected RootScenario(IApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(applicationDriver);
        ArgumentNullException.ThrowIfNull(testOutputHelper);

        _applicationDriver = applicationDriver;
        _testOutputHelper = testOutputHelper;

        Scope = applicationDriver.Services.CreateScope();

        HttpClientDriver = applicationDriver.CreateHttpClientDriver(testOutputHelper);
    }

    private IServiceScope Scope { get; }

    protected HttpClientDriver HttpClientDriver { get; }

    public virtual async Task InitializeAsync()
    {
        // Version for reset every test
        using (await Mutex.LockAsync())
        {
            await _applicationDriver.ResetStateAsync().ConfigureAwait(false);
        }
    }

    public virtual Task DisposeAsync()
    {
        Scope.Dispose();
        return Task.CompletedTask;
    }

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

    [AssertionMethod]
    protected void ThenShouldGetTheRouteOfTheNewResourceInTheHeader(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);

        runner.Execute("assert the route of the new resource is in the Location header", () =>
        {
            var headers = HttpClientDriver.ResponseMessage!.Headers;

            headers.Should().ContainKey("Location");

            var responseString = headers.GetValues("Location").Single();
            Uri.TryCreate(responseString, UriKind.RelativeOrAbsolute, out var uri).Should().BeTrue();

            runner.SetContextData(CreatedResourceLocation, uri ?? new Uri("/"));
        });
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
