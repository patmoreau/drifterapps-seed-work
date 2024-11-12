using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Refit;

// ReSharper disable CheckNamespace
namespace FluentAssertions;

/// <summary>
///     Provides assertion methods for <see cref="IApiResponse{TValue}" /> instances.
/// </summary>
public class ApiResponseAssertions<TValue>(IApiResponse<TValue> instance)
    : ReferenceTypeAssertions<IApiResponse<TValue>, ApiResponseAssertions<TValue>>(instance)
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
    ///     <see cref="ApiResponseAssertions{TValue}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<ApiResponseAssertions<TValue>> BeSuccessful(string because = "",
        params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.IsSuccessfulAssertion(Subject);

        return new AndConstraint<ApiResponseAssertions<TValue>>(this);
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
    public AndConstraint<ApiResponseAssertions<TValue>> BeFailure(string because = "", params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.IsFailureAssertion(Subject);

        return new AndConstraint<ApiResponseAssertions<TValue>>(this);
    }

    /// <summary>
    ///     Asserts that the api response has the specified value.
    /// </summary>
    /// <param name="expectedValue">The expected value.</param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ApiResponseAssertions{TValue}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<ApiResponseAssertions<TValue>> WithContent(TValue expectedValue,
        string because = "", params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.IsSuccessfulAssertion(Subject)
            .Then
            .ForCondition(Subject.Content!.Equals(expectedValue))
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:api response} to have value {0}{reason}, but found {1}.", expectedValue,
                Subject.Content);

        return new AndConstraint<ApiResponseAssertions<TValue>>(this);
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
    public AndConstraint<ApiResponseAssertions<TValue>> WithError(ApiException error, string because = "",
        params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.WithErrorAssertion(Subject, error);

        return new AndConstraint<ApiResponseAssertions<TValue>>(this);
    }
}
