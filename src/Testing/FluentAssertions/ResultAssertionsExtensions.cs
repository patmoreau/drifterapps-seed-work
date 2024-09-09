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
    /// <param name="scope">The assertion scope.</param>
    /// <param name="subject">The result to assert.</param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>A continuation for further assertions.</returns>
    internal static Continuation IsSuccessfulAssertion<TSubject>(this AssertionScope scope, TSubject subject,
        string because = "",
        params object[] becauseArgs)
        where TSubject : Result =>
        scope
            .ForCondition(subject.IsSuccess)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:result} to be a success{reason}, but it was a failure.");

    /// <summary>
    ///     Asserts that the specified result is a failure.
    /// </summary>
    /// <typeparam name="TSubject">The type of the result.</typeparam>
    /// <param name="scope">The assertion scope.</param>
    /// <param name="subject">The result to assert.</param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>A continuation for further assertions.</returns>
    internal static Continuation IsFailureAssertion<TSubject>(this AssertionScope scope, TSubject subject,
        string because = "",
        params object[] becauseArgs)
        where TSubject : Result =>
        scope
            .ForCondition(subject.IsFailure)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:result} to be a failure{reason}, but it was not.");
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
    /// <returns>An <see cref="AndConstraint{T}" /> to allow chaining assertions.</returns>
    [CustomAssertion]
    public AndConstraint<ResultAssertions> BeSuccessful(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .IsSuccessfulAssertion(Subject, because, becauseArgs);

        return new AndConstraint<ResultAssertions>(this);
    }

    /// <summary>
    ///     Asserts that the result is a failure.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>An <see cref="AndConstraint{T}" /> to allow chaining assertions.</returns>
    [CustomAssertion]
    public AndConstraint<ResultAssertions> BeFailure(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .IsFailureAssertion(Subject, because, becauseArgs);

        return new AndConstraint<ResultAssertions>(this);
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
    /// <returns>An <see cref="AndConstraint{T}" /> to allow chaining assertions.</returns>
    [CustomAssertion]
    public AndConstraint<ResultAssertions<TValue, TResult>> BeSuccessful(string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion
            .IsSuccessfulAssertion(Subject, because, becauseArgs);

        return new AndConstraint<ResultAssertions<TValue, TResult>>(this);
    }

    /// <summary>
    ///     Asserts that the result is a failure.
    /// </summary>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>An <see cref="AndConstraint{T}" /> to allow chaining assertions.</returns>
    [CustomAssertion]
    public AndConstraint<ResultAssertions<TValue, TResult>> BeFailure(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .IsFailureAssertion(Subject, because, becauseArgs);

        return new AndConstraint<ResultAssertions<TValue, TResult>>(this);
    }

    /// <summary>
    ///     Asserts that the result has the specified value.
    /// </summary>
    /// <param name="expectedValue">The expected value.</param>
    /// <param name="because">A formatted phrase explaining why the assertion is needed.</param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>An <see cref="AndConstraint{T}" /> to allow chaining assertions.</returns>
    [CustomAssertion]
    public AndConstraint<ResultAssertions<TValue, TResult>> HaveValue(TValue expectedValue,
        string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .IsSuccessfulAssertion(Subject, "a value can only be retrieved from a successful result")
            .Then
            .ForCondition(Subject.IsSuccess && Subject.Value!.Equals(expectedValue))
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:result} to have value {0}{reason}, but found {1}.", expectedValue,
                Subject.Value);

        return new AndConstraint<ResultAssertions<TValue, TResult>>(this);
    }
}
