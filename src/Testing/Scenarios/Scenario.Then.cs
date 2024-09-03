using System.Net;
using DrifterApps.Seeds.Testing.Attributes;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;

namespace DrifterApps.Seeds.Testing.Scenarios;

public abstract partial class Scenario
{
    [AssertionMethod]
    protected void ShouldNotHaveInternalServerError() =>
        ShouldHaveResponseWithStatus(statusCode => statusCode != HttpStatusCode.InternalServerError);

    [AssertionMethod]
    protected void ShouldBeAuthorizedToAccessEndpoint() =>
        HttpClientDriver.ResponseStatusCode.Should().NotBe(HttpStatusCode.Forbidden).And
            .NotBe(HttpStatusCode.Unauthorized);

    [AssertionMethod]
    protected void ShouldBeForbiddenToAccessEndpoint() =>
        HttpClientDriver.ResponseStatusCode.Should().Be(HttpStatusCode.Forbidden);

    [AssertionMethod]
    protected void ShouldNotBeAuthorizedToAccessEndpoint() =>
        HttpClientDriver.ResponseStatusCode.Should().Be(HttpStatusCode.Unauthorized);

    [AssertionMethod]
    protected TContent ShouldHaveReceived<TContent>()
    {
        var result = HttpClientDriver.DeserializeContent<TContent>();
        result.Should().NotBeNull();
        return result!;
    }

    [AssertionMethod]
    protected void ShouldExpectStatusCode(HttpStatusCode expectedStatusCode) =>
        ShouldHaveResponseWithStatus(expectedStatusCode);

    [AssertionMethod]
    protected void ShouldReceiveProblemDetailsWithErrorMessage(HttpStatusCode expectedStatusCode, string errorMessage)
    {
        ShouldExpectStatusCode(expectedStatusCode);

        var problemDetails = HttpClientDriver.DeserializeContent<ProblemDetails>();
        problemDetails.Should()
            .NotBeNull()
            .And.BeAssignableTo<ProblemDetails>()
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

    private void ShouldHaveResponseWithStatus(Func<HttpStatusCode?, bool> httpStatusPredicate) =>
        httpStatusPredicate(HttpClientDriver.ResponseStatusCode).Should().BeTrue();

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
