using DrifterApps.Seeds.Domain;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

// ReSharper disable once CheckNamespace

namespace FluentAssertions;

/// <summary>
///     Provides extension methods for asserting <see cref="Result" /> instances.
/// </summary>
public static class ResultAssertionsExtensions
{
    /// <summary>
    ///     Returns an assertion object for the specified <see cref="Result" /> instance.
    /// </summary>
    /// <param name="instance">The result instance to assert.</param>
    /// <returns>An assertion object for the specified result instance.</returns>
    public static ResultAssertions Should(this Result instance) => new(instance);

    /// <summary>
    ///     Returns an assertion object for the specified <see cref="Result{TValue}" /> instance.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained in the result.</typeparam>
    /// <param name="instance">The result instance to assert.</param>
    /// <returns>An assertion object for the specified result instance.</returns>
    public static ResultAssertions<TValue, Result<TValue>> Should<TValue>(this Result<TValue> instance) =>
        new(instance);

    /// <summary>
    ///     Asserts that the specified result is successful.
    /// </summary>
    /// <typeparam name="TSubject">The type of the result.</typeparam>
    /// <param name="assertion">The assertion scope.</param>
    /// <param name="subject">The result to assert.</param>
    /// <returns>A <see cref="Continuation" /> for further assertions.</returns>
    internal static Continuation IsSuccessfulAssertion<TSubject>(this AssertionScope assertion, TSubject subject)
        where TSubject : Result =>
        assertion
            .ForCondition(subject.IsSuccess)
            .FailWith("Expected {context:result} to be a success{reason}, but it was a failure.");

    /// <summary>
    ///     Asserts that the specified result is a failure.
    /// </summary>
    /// <typeparam name="TSubject">The type of the result.</typeparam>
    /// <param name="assertion">The assertion scope.</param>
    /// <param name="subject">The result to assert.</param>
    /// <returns>A <see cref="Continuation" /> for further assertions.</returns>
    internal static Continuation IsFailureAssertion<TSubject>(this AssertionScope assertion, TSubject subject)
        where TSubject : Result =>
        assertion
            .ForCondition(subject.IsFailure)
            .FailWith("Expected {context:result} to be a failure{reason}, but it was not.");

    /// <summary>
    ///     Asserts that the specified error is as expected.
    /// </summary>
    /// <typeparam name="TSubject">The type of the result.</typeparam>
    /// <param name="assertion">The assertion scope.</param>
    /// <param name="subject">The result to assert.</param>
    /// <param name="resultError">The <see cref="ResultError" /> to assert</param>
    /// <returns>A <see cref="Continuation" /> for further assertions.</returns>
    internal static Continuation WithErrorAssertion<TSubject>(this AssertionScope assertion,
        TSubject subject,
        ResultError resultError)
        where TSubject : Result =>
        assertion
            .ForCondition(subject.Error.Equals(resultError))
            .FailWith("Expected {context:result} to have error {0}{reason}, but found {1}.", resultError,
                subject.Error);
}

/// <summary>
///     Provides assertion methods for <see cref="Result" /> instances.
/// </summary>
public class ResultAssertions(Result instance) : ReferenceTypeAssertions<Result, ResultAssertions>(instance)
{
    /// <summary>
    ///     Gets the identifier for the assertion.
    /// </summary>
    protected override string Identifier => "result";

    /// <summary>
    ///     Asserts that the result is successful.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ResultAssertions" />
    /// </returns>
    [CustomAssertion]
    public ResultAssertions BeSuccessful(string because = "", params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.IsSuccessfulAssertion(Subject);

        return this;
    }

    /// <summary>
    ///     Asserts that the result is a failure.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ResultAssertions" />
    /// </returns>
    [CustomAssertion]
    public ResultAssertions BeFailure(string because = "", params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.IsFailureAssertion(Subject);

        return this;
    }

    /// <summary>
    ///     Asserts that the result error is as expected.
    /// </summary>
    /// <param name="resultError">The expected <see cref="ResultError" /></param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ResultAssertions" />
    /// </returns>
    [CustomAssertion]
    public ResultAssertions WithError(ResultError resultError, string because = "",
        params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.WithErrorAssertion(Subject, resultError);

        return this;
    }
}

/// <summary>
///     Provides assertion methods for <see cref="Result{TValue}" /> instances.
/// </summary>
public class ResultAssertions<TValue, TResult>(TResult instance)
    : ReferenceTypeAssertions<TResult, ResultAssertions<TValue, TResult>>(instance)
    where TResult : Result<TValue>
{
    /// <summary>
    ///     Gets the identifier for the assertion.
    /// </summary>
    protected override string Identifier => "result";

    /// <summary>
    ///     Asserts that the result is successful.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ResultAssertions{TValue, TResult}" />
    /// </returns>
    [CustomAssertion]
    public ResultAssertions<TValue, TResult> BeSuccessful(string because = "",
        params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.IsSuccessfulAssertion(Subject);

        return this;
    }

    /// <summary>
    ///     Asserts that the result is a failure.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ResultAssertions{TValue, TResult}" />
    /// </returns>
    [CustomAssertion]
    public ResultAssertions<TValue, TResult> BeFailure(string because = "", params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.IsFailureAssertion(Subject);

        return this;
    }

    /// <summary>
    ///     Asserts that the result has the specified value.
    /// </summary>
    /// <param name="expectedValue">The expected value.</param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ResultAssertions{TValue, TResult}" />
    /// </returns>
    [CustomAssertion]
    public ResultAssertions<TValue, TResult> WithValue(TValue expectedValue,
        string because = "", params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.IsSuccessfulAssertion(Subject)
            .Then
            .ForCondition(Subject.IsSuccess && Subject.Value!.Equals(expectedValue))
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:result} to have value {0}{reason}, but found {1}.", expectedValue,
                Subject.Value);

        return this;
    }

    /// <summary>
    ///     Asserts that the result error is as expected.
    /// </summary>
    /// <param name="resultError">The expected <see cref="ResultError" /></param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>
    ///     <see cref="ResultAssertions{TValue, TResult}" />
    /// </returns>
    [CustomAssertion]
    public ResultAssertions<TValue, TResult> WithError(ResultError resultError, string because = "",
        params object[] becauseArgs)
    {
        var assertion = Execute.Assertion.BecauseOf(because, becauseArgs).UsingLineBreaks;

        assertion.WithErrorAssertion(Subject, resultError);

        return this;
    }
}
