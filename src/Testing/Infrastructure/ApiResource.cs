using System.Globalization;
using System.Text.RegularExpressions;

namespace DrifterApps.Seeds.Testing.Infrastructure;

/// <summary>
///     Class to define an API endpoint for the API under test.
/// </summary>
public partial class ApiResource
{
    private ApiResource(string endpointTemplate, HttpMethod httpMethod)
    {
        EndpointTemplate = endpointTemplate;
        HttpMethod = httpMethod;
    }

    /// <summary>
    ///     Endpoint template.
    ///     The template will be used with string format to build the <see cref="Uri" /> to call.
    /// </summary>
    /// <example>api/v1/accounts/{0}</example>
    public string EndpointTemplate { get; }

    /// <summary>
    ///     True when endpoint accepts anonymous calls.
    /// </summary>
    public virtual bool IsOpen => false;

    /// <summary>
    ///     <see cref="HttpMethod" /> used for the API resource.
    /// </summary>
    public HttpMethod HttpMethod { get; }

    /// <summary>
    ///     Number of parameters expected from the <see cref="EndpointTemplate" />
    /// </summary>
    public int ParameterCount => ParametersRegex().Matches(EndpointTemplate).Count;

    /// <summary>
    ///     Helper method to define an <see cref="ApiResource" />.
    /// </summary>
    /// <param name="endpointTemplate">Endpoint template which will be used with string format to build the <see cref="Uri" />.</param>
    /// <param name="httpMethod"><see cref="HttpMethod" /> used for this API resource.</param>
    /// <returns><see cref="ApiResource" /> created</returns>
    public static ApiResource DefineApi(string endpointTemplate, HttpMethod httpMethod) =>
        new(endpointTemplate, httpMethod);

    public static ApiResource DefineOpenApi(string endpointTemplate, HttpMethod httpMethod) =>
        new OpenApiResource(endpointTemplate, httpMethod);

    internal Uri EndpointFromResource() => new(EndpointTemplate, UriKind.Relative);

    internal Uri EndpointFromResource(params object[] parameters) =>
        new(string.Format(CultureInfo.InvariantCulture, EndpointTemplate, parameters), UriKind.Relative);

    [GeneratedRegex("\\{\\d+\\}")]
    private static partial Regex ParametersRegex();

    private sealed class OpenApiResource : ApiResource
    {
        public OpenApiResource(string endpointTemplate, HttpMethod httpMethod) : base(endpointTemplate, httpMethod)
        {
        }

        public override bool IsOpen => true;
    }
}
