using System.Net;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Refit;

namespace DrifterApps.Seeds.Testing.FluentAssertions;

public abstract class BaseApiResponseAssertions<TValue, TAssertions>(TValue instance) :
    ReferenceTypeAssertions<TValue, TAssertions>(instance)
    where TAssertions : ReferenceTypeAssertions<TValue, TAssertions>
    where TValue : IApiResponse
{
    /// <summary>
    ///     Asserts that the api response is successful.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="AndConstraint{TAssertions}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<TAssertions> BeSuccessful(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .UsingLineBreaks
            .ForCondition(Subject.IsSuccessful)
            .FailWith(Reason("Expected {context:response} to be successful{reason}, but it was not."));

        return new AndConstraint<TAssertions>((TAssertions)(object)this);
    }

    /// <summary>
    ///     Asserts that the api response is a failure.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="AndConstraint{TAssertions}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<TAssertions> BeFailure(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .UsingLineBreaks
            .ForCondition(!Subject.IsSuccessful)
            .FailWith(Reason("Expected {context:response} to be a failure{reason}, but it was not."));

        return new AndConstraint<TAssertions>((TAssertions)(object)this);
    }

    /// <summary>
    ///     Asserts that the api response is authorized.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="AndConstraint{TAssertions}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<TAssertions> BeAuthorized(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .UsingLineBreaks
            .ForCondition(Subject.StatusCode is not HttpStatusCode.Forbidden and not HttpStatusCode.Unauthorized)
            .FailWith(Reason("Expected {context:response} to be authorized{reason}, but {0} was not.",
                Subject.StatusCode));

        return new AndConstraint<TAssertions>((TAssertions)(object)this);
    }

    /// <summary>
    ///     Asserts that the api response is forbidden.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="AndConstraint{TAssertions}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<TAssertions> BeForbidden(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .UsingLineBreaks
            .ForCondition(Subject.StatusCode is HttpStatusCode.Forbidden)
            .FailWith(Reason("Expected {context:response} to be forbidden{reason}, but {0} was not.",
                Subject.StatusCode));

        return new AndConstraint<TAssertions>((TAssertions)(object)this);
    }

    /// <summary>
    ///     Asserts that the api response is not authorized.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="AndConstraint{TAssertions}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<TAssertions> NotBeAuthorized(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .UsingLineBreaks
            .ForCondition(Subject.StatusCode is HttpStatusCode.Unauthorized)
            .FailWith(Reason("Expected {context:response} not to be authorized{reason}, but it was {0}.", Subject.StatusCode));

        return new AndConstraint<TAssertions>((TAssertions)(object)this);
    }

    /// <summary>
    ///     Asserts that the api response has status code.
    /// </summary>
    /// <param name="statusCode"><see cref="HttpStatusCode" /> to assert</param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="AndConstraint{TAssertions}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<TAssertions> HaveStatusCode(HttpStatusCode statusCode, string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .UsingLineBreaks
            .ForCondition(Subject.StatusCode == statusCode)
            .FailWith(Reason("Expected {context:response} to have status code {0}{reason}, but it was {1}.", statusCode,
                Subject.StatusCode));

        return new AndConstraint<TAssertions>((TAssertions)(object)this);
    }

    /// <summary>
    ///     Asserts that the api response doest not have status code.
    /// </summary>
    /// <param name="statusCode"><see cref="HttpStatusCode" /> to assert</param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="AndConstraint{TAssertions}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<TAssertions> NotHaveStatusCode(HttpStatusCode statusCode, string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .UsingLineBreaks
            .ForCondition(Subject.StatusCode != statusCode)
            .FailWith(Reason("Expected {context:response} to not have status code {0}{reason}, but it was.", statusCode,
                Subject.StatusCode));

        return new AndConstraint<TAssertions>((TAssertions)(object)this);
    }

    /// <summary>
    ///     Asserts that the api response error is as expected.
    /// </summary>
    /// <param name="error">The expected <see cref="ApiException" /></param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="AndConstraint{TAssertions}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<TAssertions> HaveError(string error, string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .UsingLineBreaks
            .ForCondition(Subject.Error is not null &&
                          (Subject.Error.Content?.Contains(error, StringComparison.InvariantCulture) ?? false))
            .FailWith(Reason("Expected {context:response} to have error {0}{reason}, but found {1}.", error,
                Subject.Error?.Content!));

        return new AndConstraint<TAssertions>((TAssertions)(object)this);
    }

    /// <summary>
    ///     Asserts that the api response has a location header.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="AndConstraint{TAssertions}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<TAssertions> HaveLocation(string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .UsingLineBreaks
            .ForCondition(Subject.Headers.Location is not null)
            .FailWith(Reason("Expected {context:response} to have Location header{reason}, but it did not."));

        return new AndConstraint<TAssertions>((TAssertions)(object)this);
    }

    private Func<FailReason> Reason(string message, params object[] args) =>
        () => HasMonitoringMessage(out var monitoringMessage)
            ? new FailReason(message + $"{{{args.Length}}}", [.. args, .. new[] { monitoringMessage! }])
            : new FailReason(message, args);

    private bool HasMonitoringMessage(out object? monitoringMessage)
    {
        var requestCorrelationId = string.Empty;
        var responseCorrelationId = string.Empty;

        if (Subject.RequestMessage is not null &&
            Subject.RequestMessage.Headers.TryGetValues("X-Correlation-ID", out var requestCorrelationIds))
        {
            requestCorrelationId = requestCorrelationIds.FirstOrDefault();
        }

        if (Subject.Headers.TryGetValues("X-Correlation-ID", out var responseCorrelationIds))
        {
            responseCorrelationId = responseCorrelationIds.FirstOrDefault();
        }

        if (string.IsNullOrWhiteSpace(requestCorrelationId) && string.IsNullOrWhiteSpace(responseCorrelationId))
        {
            monitoringMessage = null;
            return false;
        }

        monitoringMessage = new
        {
            Request = new
            {
                Subject.RequestMessage?.RequestUri,
                XCorrelationId = requestCorrelationId
            },
            Response = new { XCorrelationId = responseCorrelationId }
        };
        return true;
    }
}
