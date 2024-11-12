using System.Net;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Refit;

// ReSharper disable CheckNamespace
namespace FluentAssertions;

/// <summary>
///     Provides assertion methods for <see cref="IApiResponse" /> instances.
/// </summary>
public class ApiResponseAssertions(IApiResponse instance)
    : ReferenceTypeAssertions<IApiResponse, ApiResponseAssertions>(instance)
{
    /// <summary>
    ///     Gets the identifier for the assertion.
    /// </summary>
    protected override string Identifier => "ApiResponse";

    /// <summary>
    ///     Asserts that the api response is successful.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ApiResponseAssertions" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<ApiResponseAssertions> BeSuccessful(string because = "",
        params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.IsSuccessfulAssertion(Subject);

        return new AndConstraint<ApiResponseAssertions>(this);
    }

    /// <summary>
    ///     Asserts that the api response is a failure.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ApiResponseAssertions{TValue}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<ApiResponseAssertions> BeFailure(string because = "", params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.IsFailureAssertion(Subject);

        return new AndConstraint<ApiResponseAssertions>(this);
    }

    /// <summary>
    ///     Asserts that the api response has status code.
    /// </summary>
    /// <param name="statusCode"><see cref="HttpStatusCode" /> to assert</param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ApiResponseAssertions{TValue}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<ApiResponseAssertions> HaveStatusCode(HttpStatusCode statusCode, string because = "",
        params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.HaveStatusCodeAssertion(Subject, statusCode);

        return new AndConstraint<ApiResponseAssertions>(this);
    }

    /// <summary>
    ///     Asserts that the api response doest not have status code.
    /// </summary>
    /// <param name="statusCode"><see cref="HttpStatusCode" /> to assert</param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ApiResponseAssertions{TValue}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<ApiResponseAssertions> NotHaveStatusCode(HttpStatusCode statusCode, string because = "",
        params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.NotHaveStatusCodeAssertion(Subject, statusCode);

        return new AndConstraint<ApiResponseAssertions>(this);
    }

    /// <summary>
    ///     Asserts that the api response error is as expected.
    /// </summary>
    /// <param name="error">The expected <see cref="ApiException" /></param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ApiResponseAssertions{TValue}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<ApiResponseAssertions> WithError(ApiException error, string because = "",
        params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.WithErrorAssertion(Subject, error);

        return new AndConstraint<ApiResponseAssertions>(this);
    }
}
