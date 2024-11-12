using System.Net;
using DrifterApps.Seeds.Infrastructure;
using DrifterApps.Seeds.Testing.Attributes;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Refit;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace DrifterApps.Seeds.Testing.Scenarios;

public abstract partial class Scenario
{
    [AssertionMethod]
    protected void ShouldNotHaveInternalServerError() =>
        _runner?.GetContextData<IApiResponse>(ContextHttpResponse)
            .Should().NotBeNull()
            .And.NotHaveStatusCode(HttpStatusCode.InternalServerError);

    [AssertionMethod]
    protected void ShouldBeAuthorizedToAccessEndpoint() =>
        _runner?.GetContextData<IApiResponse>(ContextHttpResponse)
            .Should().NotBeNull()
            .And.NotHaveStatusCode(HttpStatusCode.Forbidden)
            .And.NotHaveStatusCode(HttpStatusCode.Unauthorized);

    [AssertionMethod]
    protected void ShouldBeForbiddenToAccessEndpoint() =>
        _runner?.GetContextData<IApiResponse>(ContextHttpResponse)
            .Should().NotBeNull()
            .And.HaveStatusCode(HttpStatusCode.Forbidden);

    [AssertionMethod]
    protected void ShouldNotBeAuthorizedToAccessEndpoint() =>
        _runner?.GetContextData<IApiResponse>(ContextHttpResponse)
            .Should().NotBeNull()
            .And.HaveStatusCode(HttpStatusCode.Unauthorized);

    [AssertionMethod]
    protected void ShouldExpectStatusCode(HttpStatusCode expectedStatusCode) =>
        ShouldHaveResponseWithStatus(expectedStatusCode);

    [AssertionMethod]
    protected void ShouldReceiveProblemDetailsWithErrorMessage(HttpStatusCode expectedStatusCode, string errorMessage)
    {
        ShouldExpectStatusCode(expectedStatusCode);

        _runner?.GetContextData<IApiResponse>(ContextHttpResponse)
            .Should().NotBeNull()
            .And.Subject.Error.ToProblemDetails()
            .Should().NotBeNull().And.BeAssignableTo<ProblemDetails>()
            .Subject.Detail.Should().Be(errorMessage);
    }

    [AssertionMethod]
    protected void ShouldReceiveValidationProblemDetailsWithErrorMessage(string errorMessage,
        HttpStatusCode expectedStatusCode = HttpStatusCode.UnprocessableEntity)
    {
        ShouldExpectStatusCode(expectedStatusCode);

        var problemDetails = HttpClientDriver.DeserializeContent<ValidationProblemDetails>();
        problemDetails.Should()
            .NotBeNull()
            .And.BeAssignableTo<ValidationProblemDetails>()
            .Subject.Title.Should()
            .Be(errorMessage);
    }

    [AssertionMethod]
    protected Uri ShouldGetTheRouteOfTheNewResourceInTheHeader()
    {
        HttpClientDriver.ResponseLocation.Should().NotBeNull();

        return HttpClientDriver.ResponseLocation!;
    }

    private void ShouldHaveResponseWithStatus(HttpStatusCode httpStatus)
    {
        _runner?.GetContextData<IApiResponse>(ContextHttpResponse)
            .Should().NotBeNull()
            .And.HaveStatusCode(httpStatus);

        if (HttpClientDriver.ResponseStatusCode != httpStatus)
        {
            var content = string.IsNullOrWhiteSpace(HttpClientDriver.ResponseContent)
                ? "<empty response>"
                : HttpClientDriver.ResponseContent;
            TestOutputHelper.WriteLine(
                $"Unexpected HTTP {HttpClientDriver.ResponseStatusCode} Code with Response: {content}");
        }

        HttpClientDriver.ResponseStatusCode.Should().Be(httpStatus);
    }

#pragma warning disable CA1822
    [AssertionMethod]
    protected void AssertAll(Func<Task> assertions)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        using AssertionScope scope = new();
        assertions();
    }

    [AssertionMethod]
    protected void AssertAll(Action assertions)
    {
        ArgumentNullException.ThrowIfNull(assertions);

        using AssertionScope scope = new();
        assertions();
    }
#pragma warning restore CA1822
}
