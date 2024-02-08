using System.Net;
using DrifterApps.Seeds.Testing.Attributes;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using static DrifterApps.Seeds.Testing.Drivers.HttpClientDriver;

namespace DrifterApps.Seeds.Testing.Scenarios;

public abstract partial class Scenario
{
    [AssertionMethod]
    protected void ShouldNotHaveInternalServerError() =>
        ShouldHaveResponseWithStatus(statusCode => statusCode != HttpStatusCode.InternalServerError);

    [AssertionMethod]
    protected void ShouldBeAuthorizedToAccessEndpoint() =>
        HttpClientDriver.ResponseMessage.Should().NotBeNull().And.NotHaveStatusCode(HttpStatusCode.Forbidden).And
            .NotHaveStatusCode(HttpStatusCode.Unauthorized);

    [AssertionMethod]
    protected void ShouldBeForbiddenToAccessEndpoint() =>
        HttpClientDriver.ResponseMessage.Should().NotBeNull().And.HaveStatusCode(HttpStatusCode.Forbidden);

    [AssertionMethod]
    protected void ShouldNotBeAuthorizedToAccessEndpoint() =>
        HttpClientDriver.ResponseMessage.Should().NotBeNull().And.HaveStatusCode(HttpStatusCode.Unauthorized);

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
    protected void ShouldReceiveValidationProblemDetailsWithErrorMessage(string errorMessage)
    {
        ShouldExpectStatusCode(HttpStatusCode.UnprocessableEntity);

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
        var headers = HttpClientDriver.ResponseMessage!.Headers;
        headers.Should().ContainKey("Location");

        var locations = headers.GetValues("Location").ToList();
        locations.Should().HaveCount(1);

        Uri.TryCreate(locations[0], UriKind.RelativeOrAbsolute, out var uri).Should().BeTrue();

        return uri!;
    }

    private void ShouldHaveResponseWithStatus(HttpStatusCode httpStatus)
    {
        LogUnexpectedContent(HttpClientDriver.ResponseMessage, httpStatus, TestOutputHelper);

        HttpClientDriver.ResponseMessage.Should().NotBeNull().And.HaveStatusCode(httpStatus);
    }

    private void ShouldHaveResponseWithStatus(Func<HttpStatusCode?, bool> httpStatusPredicate)
    {
        ArgumentNullException.ThrowIfNull(httpStatusPredicate);

        HttpClientDriver.ResponseMessage.Should().NotBeNull();
        httpStatusPredicate(HttpClientDriver.ResponseMessage!.StatusCode).Should().BeTrue();
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
