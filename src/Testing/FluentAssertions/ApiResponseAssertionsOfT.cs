using DrifterApps.Seeds.Testing.FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using Refit;

#pragma warning disable CS1587 // XML comment is not placed on a valid language element
#pragma warning disable S125 // Sections of code should not be commented out

// ReSharper disable CheckNamespace
namespace FluentAssertions;

/// <summary>
///     Provides assertion methods for <see cref="IApiResponse{TValue}" /> instances.
/// </summary>
public class ApiResponseAssertions<TValue>(IApiResponse<TValue> instance, AssertionChain assertionChain)
    : BaseApiResponseAssertions<IApiResponse<TValue>, ApiResponseAssertions<TValue>>(instance, assertionChain)
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
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .UsingLineBreaks
            .ForCondition(HasContentCondition())
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
    public AndConstraint<ApiResponseAssertions<TValue>> HaveContent(TValue expectedValue,
        string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .UsingLineBreaks
            .ForCondition(HasContentCondition())
            .FailWith("Expected {context:response} to have content{reason}, but found nothing.");

        Subject.Content.Should().Be(expectedValue, because, becauseArgs);

        return new AndConstraint<ApiResponseAssertions<TValue>>(this);
    }

    /// <summary>
    ///     Asserts that the api response has the equivalent value.
    /// </summary>
    /// <param name="expectedValue">The expected value.</param>
    /// <param name="config">A function to configure the equivalency assertion options.</param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ApiResponseAssertions{TValue}" />
    /// </returns>
    [CustomAssertion]
    public AndConstraint<ApiResponseAssertions<TValue>> HaveEquivalentContent(
        TValue expectedValue,
        Func<EquivalencyOptions<TValue>, EquivalencyOptions<TValue>>? config = null,
        string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .UsingLineBreaks
            .ForCondition(HasContentCondition())
            .FailWith("Expected {context:response} to have content{reason}, but found nothing.");

        Subject.Content.Should().BeEquivalentTo(expectedValue, config ?? (options => options), because, becauseArgs);

        return new AndConstraint<ApiResponseAssertions<TValue>>(this);
    }

    private bool HasContentCondition() => !(Subject.Content is null ||
                                          (Subject.ContentHeaders!.Contains("Content-Length") &&
                                           Subject.ContentHeaders!.ContentLength == 0));
}
