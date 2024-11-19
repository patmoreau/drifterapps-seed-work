using System.Net;
using System.Text.Json;
using DrifterApps.Seeds.Testing.Attributes;
using DrifterApps.Seeds.Testing.Extensions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Refit;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace DrifterApps.Seeds.Testing.Scenarios;

public abstract partial class Scenario
{
    [AssertionMethod]
    protected void ShouldNotHaveInternalServerError() =>
        Runner.ApiResponse<IApiResponse>()
            .Should().NotHaveStatusCode(HttpStatusCode.InternalServerError);

    [AssertionMethod]
    protected void ShouldBeAuthorizedToAccessEndpoint() =>
        Runner.ApiResponse<IApiResponse>()
            .Should().NotHaveStatusCode(HttpStatusCode.Forbidden)
            .And.NotHaveStatusCode(HttpStatusCode.Unauthorized);

    [AssertionMethod]
    protected void ShouldBeForbiddenToAccessEndpoint() =>
        Runner.ApiResponse<IApiResponse>()
            .Should().HaveStatusCode(HttpStatusCode.Forbidden);

    [AssertionMethod]
    protected void ShouldNotBeAuthorizedToAccessEndpoint() =>
        Runner.ApiResponse<IApiResponse>()
            .Should().HaveStatusCode(HttpStatusCode.Unauthorized);

    [AssertionMethod]
    protected void ShouldExpectStatusCode(HttpStatusCode expectedStatusCode) =>
        ShouldHaveResponseWithStatus(expectedStatusCode);

    [AssertionMethod]
    protected void ShouldReceiveProblemDetailsWithErrorMessage(HttpStatusCode expectedStatusCode, string errorMessage)
    {
        ShouldExpectStatusCode(expectedStatusCode);

        var response = Runner.ApiResponse<IApiResponse>();
        response
            .Error.Should().NotBeNull()
            .And.BeAssignableTo<ApiException>()
            .And.As<ApiException>()
            .Content.Should().NotBeNull();

        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(response.Error!.Content!);
        problemDetails.Should()
            .NotBeNull()
            .And.BeAssignableTo<ProblemDetails>()
            .Subject.Detail.Should()
            .Be(errorMessage);
    }

    [AssertionMethod]
    protected void ShouldReceiveValidationProblemDetailsWithErrorMessage(string errorMessage,
        HttpStatusCode expectedStatusCode = HttpStatusCode.UnprocessableEntity)
    {
        ShouldExpectStatusCode(expectedStatusCode);

        var response = Runner.ApiResponse<IApiResponse>();
        response
            .Error.Should().NotBeNull()
            .And.BeAssignableTo<ApiException>()
            .And.As<ApiException>()
            .Content.Should().NotBeNull();

        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(response.Error!.Content!);
        problemDetails.Should()
            .NotBeNull()
            .And.BeAssignableTo<ProblemDetails>()
            .Subject.Title.Should()
            .Be(errorMessage);
    }

    [AssertionMethod]
    protected Uri ShouldGetTheRouteOfTheNewResourceInTheHeader()
    {
        var response = Runner.ApiResponse<IApiResponse>();
        response
            .Should()
            .HaveStatusCode(HttpStatusCode.Created)
            .And.HaveLocation();

        return response.Headers.Location!;
    }

    private void ShouldHaveResponseWithStatus(HttpStatusCode httpStatus) =>
        Runner.ApiResponse<IApiResponse>()
            .Should().NotBeNull()
            .And.HaveStatusCode(httpStatus);

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
