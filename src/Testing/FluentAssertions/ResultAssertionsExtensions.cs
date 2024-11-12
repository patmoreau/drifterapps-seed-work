using System.Net;
using FluentAssertions.Execution;
using Refit;

// ReSharper disable once CheckNamespace
namespace FluentAssertions;

/// <summary>
///     Provides extension methods for asserting <see cref="ApiResponse{T}" /> instances.
/// </summary>
public static class ApiResponseAssertionsExtensions
{
    /// <summary>
    ///     Returns an assertion object for the specified <see cref="ApiResponse{TValue}" /> instance.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained in the api response.</typeparam>
    /// <param name="instance">The api response instance to assert.</param>
    /// <returns>An assertion object for the specified api response instance.</returns>
    public static ApiResponseAssertions<TValue> Should<TValue>(this ApiResponse<TValue> instance) => new(instance);

    /// <summary>
    ///     Returns an assertion object for the specified <see cref="ApiResponse{TValue}" /> instance.
    /// </summary>
    /// <param name="instance">The api response instance to assert.</param>
    /// <returns>An assertion object for the specified api response instance.</returns>
    public static ApiResponseAssertions Should(this IApiResponse instance) => new(instance);

    /// <summary>
    ///     Asserts that the specified api response is successful.
    /// </summary>
    /// <param name="assertion">The assertion scope.</param>
    /// <param name="subject">The api response to assert.</param>
    /// <returns>A <see cref="Continuation" /> for further assertions.</returns>
    internal static Continuation IsSuccessfulAssertion(this AssertionScope assertion,
        IApiResponse subject) =>
        assertion
            .ForCondition(subject.IsSuccessful)
            .FailWith("Expected {context:response} to be a success{reason}, but it was not.");

    /// <summary>
    ///     Asserts that the specified api response is a failure.
    /// </summary>
    /// <param name="assertion">The assertion scope.</param>
    /// <param name="subject">The api response to assert.</param>
    /// <returns>A <see cref="Continuation" /> for further assertions.</returns>
    internal static Continuation IsFailureAssertion(this AssertionScope assertion, IApiResponse subject) =>
        assertion
            .ForCondition(!subject.IsSuccessStatusCode)
            .FailWith("Expected {context:api response} to be a failure{reason}, but it was not.");

    /// <summary>
    ///     Asserts that the specified api response has status code.
    /// </summary>
    /// <param name="assertion">The assertion scope.</param>
    /// <param name="subject">The api response to assert.</param>
    /// <param name="statusCode"></param>
    /// <returns>A <see cref="Continuation" /> for further assertions.</returns>
    internal static Continuation HaveStatusCodeAssertion(this AssertionScope assertion, IApiResponse subject,
        HttpStatusCode statusCode) =>
        assertion
            .ForCondition(subject.StatusCode == statusCode)
            .FailWith("Expected {context:response} to have status code{0}{reason}, but it was {1}.", statusCode,
                subject.StatusCode);

    /// <summary>
    ///     Asserts that the specified api response does not have status code.
    /// </summary>
    /// <param name="assertion">The assertion scope.</param>
    /// <param name="subject">The api response to assert.</param>
    /// <param name="statusCode"></param>
    /// <returns>A <see cref="Continuation" /> for further assertions.</returns>
    internal static Continuation NotHaveStatusCodeAssertion(this AssertionScope assertion, IApiResponse subject,
        HttpStatusCode statusCode) =>
        assertion
            .ForCondition(subject.StatusCode != statusCode)
            .FailWith("Expected {context:response} to not have status code{0}{reason}, but it was.", statusCode,
                subject.StatusCode);

    /// <summary>
    ///     Asserts that the specified error is as expected.
    /// </summary>
    /// <param name="assertion">The assertion scope.</param>
    /// <param name="subject">The api response to assert.</param>
    /// <param name="error">The <see cref="ApiException" /> to assert</param>
    /// <returns>A <see cref="Continuation" /> for further assertions.</returns>
    internal static Continuation WithErrorAssertion(this AssertionScope assertion, IApiResponse subject,
        ApiException error) =>
        assertion
            .ForCondition(subject.Error is not null && subject.Error.Equals(error))
            .FailWith("Expected {context:response} to have error {0}{reason}, but found {1}.", error, subject.Error);
}
