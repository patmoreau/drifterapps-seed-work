using DrifterApps.Seeds.Testing.AwesomeAssertions;
using Refit;

// ReSharper disable CheckNamespace
namespace AwesomeAssertions;

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
