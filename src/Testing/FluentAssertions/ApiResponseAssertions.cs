using DrifterApps.Seeds.Testing.FluentAssertions;
using FluentAssertions.Execution;
using Refit;

// ReSharper disable CheckNamespace
namespace FluentAssertions;

/// <summary>
///     Provides assertion methods for <see cref="IApiResponse" /> instances.
/// </summary>
public class ApiResponseAssertions(IApiResponse instance, AssertionChain assertionChain)
    : BaseApiResponseAssertions<IApiResponse, ApiResponseAssertions>(instance, assertionChain)
{
    /// <summary>
    ///     Gets the identifier for the assertion.
    /// </summary>
    protected override string Identifier => "ApiResponse";
}
