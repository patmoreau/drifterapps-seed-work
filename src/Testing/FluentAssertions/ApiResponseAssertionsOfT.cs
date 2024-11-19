using DrifterApps.Seeds.Testing.FluentAssertions;
using FluentAssertions.Execution;
using Refit;

#pragma warning disable CS1587 // XML comment is not placed on a valid language element
#pragma warning disable S125 // Sections of code should not be commented out

// ReSharper disable CheckNamespace
namespace FluentAssertions;

/// <summary>
///     Provides assertion methods for <see cref="IApiResponse{TValue}" /> instances.
/// </summary>
public class ApiResponseAssertions<TValue>(IApiResponse<TValue> instance)
    : BaseApiResponseAssertions<IApiResponse<TValue>, ApiResponseAssertions<TValue>>(instance)
{
    /// <summary>
    ///     Gets the identifier for the assertion.
    /// </summary>
    protected override string Identifier => "ApiResponse";

    /// <summary>
    ///     Asserts that the api response has the specified value.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ApiResponseAssertions{TValue}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<ApiResponseAssertions<TValue>> HaveContent(string because = "", params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.ForCondition(Subject.Content is not null)
            .FailWith("Expected {context:response} to have content{reason}, but found nothing.");

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

        assertion.ForCondition(Subject.Content!.Equals(expectedValue))
            .FailWith("Expected {context:response} to have value {0}{reason}, but found {1}.", expectedValue,
                Subject.Content);

        return new AndConstraint<ApiResponseAssertions<TValue>>(this);
    }
}
