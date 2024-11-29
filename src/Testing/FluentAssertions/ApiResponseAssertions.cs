using DrifterApps.Seeds.Testing.FluentAssertions;
using Refit;

// ReSharper disable CheckNamespace
namespace FluentAssertions;

/// <summary>
///     Provides assertion methods for <see cref="IApiResponse" /> instances.
/// </summary>
public class ApiResponseAssertions(IApiResponse instance)
    : BaseApiResponseAssertions<IApiResponse, ApiResponseAssertions>(instance)
{
    /// <summary>
    ///     Gets the identifier for the assertion.
    /// </summary>
    protected override string Identifier => "ApiResponse";
}
