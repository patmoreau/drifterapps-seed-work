using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DrifterApps.Seeds.Testing.Infrastructure;
using DrifterApps.Seeds.Testing.Infrastructure.Authentication;
using Xunit.Abstractions;

namespace DrifterApps.Seeds.Testing.Drivers;

public sealed class HttpClientDriver : IHttpClientDriver, IDisposable
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() {PropertyNameCaseInsensitive = true};

    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _testOutputHelper;

    private HttpClientDriver(HttpClient httpClient, ITestOutputHelper testOutputHelper)
    {
        _httpClient = httpClient;
        _testOutputHelper = testOutputHelper;
    }

    public void Dispose() => ResponseMessage?.Dispose();

    public HttpResponseMessage? ResponseMessage { get; private set; }

    public async Task<T?> DeserializeContentAsync<T>()
    {
        if (ResponseMessage is null) return default;
        var resultAsString = await ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(resultAsString)) return default;

        var content = JsonSerializer.Deserialize<T>(resultAsString, JsonSerializerOptions);
        return content;
    }

    public void AuthenticateUser(string userId) =>
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(MockAuthenticationHandler.AuthenticationScheme, userId);

    public void UnAuthenticate() => _httpClient.DefaultRequestHeaders.Authorization = null;

    public async Task SendRequestAsync(ApiResource apiResource, string? query = null, string? body = null,
        params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(apiResource);

        var baseUri = apiResource.EndpointFromResource(parameters);
        var fullUri = baseUri;
        if (query is not null)
            fullUri = new Uri($"{fullUri}?{query}", fullUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);

        using var requestMessage = new HttpRequestMessage(apiResource.HttpMethod, fullUri);
        requestMessage.Content = body is not null ? new StringContent(body, Encoding.UTF8, "application/json") : null;

        ResponseMessage = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

        await LogUnexpectedErrors(ResponseMessage, _testOutputHelper).ConfigureAwait(false);
    }

    public static IHttpClientDriver CreateDriver(HttpClient httpClient, ITestOutputHelper testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        return new HttpClientDriver(httpClient, testOutputHelper);
    }

    internal static async Task LogUnexpectedErrors(HttpResponseMessage? responseMessage,
        ITestOutputHelper testOutputHelper)
    {
        if (responseMessage is null) return;

        if (responseMessage.StatusCode != HttpStatusCode.InternalServerError) return;

        var resultAsString = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

        var content = string.IsNullOrWhiteSpace(resultAsString) ? "<empty response>" : resultAsString;
        testOutputHelper.WriteLine($"HTTP 500 Response: {content}");
    }

    internal static async Task LogUnexpectedContent(HttpResponseMessage? responseMessage,
        HttpStatusCode expectedStatusCode,
        ITestOutputHelper testOutputHelper)
    {
        if (responseMessage is null) return;

        if (responseMessage.StatusCode == expectedStatusCode) return;

        var resultAsString = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

        var content = string.IsNullOrWhiteSpace(resultAsString) ? "<empty response>" : resultAsString;
        testOutputHelper.WriteLine($"Unexpected HTTP {responseMessage.StatusCode} Code with Response: {content}");
    }
}
