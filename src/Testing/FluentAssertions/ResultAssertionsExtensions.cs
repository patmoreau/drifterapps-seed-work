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
    /// <typeparam name="TValue">The type of the value contained in the api response.</typeparam>
    /// <param name="instance">The api response instance to assert.</param>
    /// <returns>An assertion object for the specified api response instance.</returns>
    public static ApiResponseAssertions<TValue> Should<TValue>(this IApiResponse<TValue> instance) => new(instance);

    /// <summary>
    ///     Returns an assertion object for the specified <see cref="ApiResponse{TValue}" /> instance.
    /// </summary>
    /// <param name="instance">The api response instance to assert.</param>
    /// <returns>An assertion object for the specified api response instance.</returns>
    public static ApiResponseAssertions Should(this IApiResponse instance) => new(instance);
}
