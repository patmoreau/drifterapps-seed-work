using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DrifterApps.Seeds.Testing.Infrastructure;
using DrifterApps.Seeds.Testing.Infrastructure.Authentication;
using Xunit.Abstractions;

namespace DrifterApps.Seeds.Testing.Drivers;

public sealed class HttpClientDriver : IHttpClientDriver
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() {PropertyNameCaseInsensitive = true};

    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _testOutputHelper;

    private HttpClientDriver(HttpClient httpClient, ITestOutputHelper testOutputHelper)
    {
        _httpClient = httpClient;
        _testOutputHelper = testOutputHelper;
    }

    public HttpResponseMessage? ResponseMessage { get; private set; }

    public T? DeserializeContent<T>()
    {
        var resultAsString = ResponseMessage?.Content.ReadAsStringAsync().Result;
        if (resultAsString is null) return default;

        var content = JsonSerializer.Deserialize<T>(resultAsString, JsonSerializerOptions);
        return content;
    }

    public void AuthenticateUser(string userId) =>
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(MockAuthenticationHandler.AuthenticationScheme, userId);

    public void UnAuthenticate() => _httpClient.DefaultRequestHeaders.Authorization = null;

    public async Task SendGetRequestAsync(ApiResource apiResource, string? query = null)
    {
        ArgumentNullException.ThrowIfNull(apiResource);

        var baseUri = apiResource.EndpointFromResource();
        var fullUri = baseUri;
        if (query is not null)
            fullUri = new Uri($"{fullUri}?{query}", fullUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);

        using HttpRequestMessage request = new(HttpMethod.Get, fullUri);
        await SendRequest(request).ConfigureAwait(false);
    }

    public async Task SendGetRequestAsync(ApiResource apiResource, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(apiResource);

        var endpointUri = apiResource.EndpointFromResource(parameters);
        using HttpRequestMessage request = new(HttpMethod.Get, endpointUri);
        await SendRequest(request).ConfigureAwait(false);
    }

    public async Task SendPostRequestAsync(ApiResource apiResource, string? body = null)
    {
        ArgumentNullException.ThrowIfNull(apiResource);

        var endpointUri = apiResource.EndpointFromResource();

        using HttpRequestMessage request = new(HttpMethod.Post, endpointUri);
        if (body is not null) request.Content = new StringContent(body, Encoding.UTF8, "application/json");

        await SendRequest(request).ConfigureAwait(false);
    }

    public async Task SendDeleteRequestAsync(ApiResource apiResource, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(apiResource);

        var endpointUri = apiResource.EndpointFromResource(parameters);
        using HttpRequestMessage request = new(HttpMethod.Delete, endpointUri);
        await SendRequest(request).ConfigureAwait(false);
    }

    private async Task SendRequest(HttpRequestMessage requestMessage)
    {
        ResponseMessage = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

        LogUnexpectedErrors(ResponseMessage, _testOutputHelper);
    }

    public static IHttpClientDriver CreateDriver(HttpClient httpClient, ITestOutputHelper testOutputHelper) =>
        new HttpClientDriver(httpClient, testOutputHelper);

    internal static void LogUnexpectedErrors(HttpResponseMessage? responseMessage, ITestOutputHelper testOutputHelper)
    {
        if (responseMessage is null) return;

        if (responseMessage.StatusCode != HttpStatusCode.InternalServerError) return;

        var resultAsString = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        var content = string.IsNullOrWhiteSpace(resultAsString) ? "<empty response>" : resultAsString;
        testOutputHelper.WriteLine($"HTTP 500 Response: {content}");
    }

    internal static void LogUnexpectedContent(HttpResponseMessage? responseMessage, HttpStatusCode expectedStatusCode,
        ITestOutputHelper testOutputHelper)
    {
        if (responseMessage is null) return;

        if (responseMessage.StatusCode == expectedStatusCode) return;

        var resultAsString = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        var content = string.IsNullOrWhiteSpace(resultAsString) ? "<empty response>" : resultAsString;
        testOutputHelper.WriteLine($"Unexpected HTTP {responseMessage.StatusCode} Code with Response: {content}");
    }
}
