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

    public HttpStatusCode ResponseStatusCode { get; private set; }

    public string? ResponseContent { get; private set; }

    public Uri? ResponseLocation { get; private set; }

    public T? DeserializeContent<T>()
    {
        if (string.IsNullOrWhiteSpace(ResponseContent)) return default;

        var content = JsonSerializer.Deserialize<T>(ResponseContent, JsonSerializerOptions);
        return content;
    }

    public void AuthenticateUser(string userId) =>
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(MockAuthenticationHandler.AuthenticationScheme, userId);

    public void UnAuthenticate() => _httpClient.DefaultRequestHeaders.Authorization = null;

    public Task SendRequestAsync(ApiResource apiResource, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(apiResource);

        return SendHttpRequestMessageAsync(apiResource, null, parameters);
    }

    public Task SendRequestWithBodyAsync(ApiResource apiResource, string body, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(apiResource);
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        ArgumentException.ThrowIfNullOrEmpty(body?.Trim());

        return SendHttpRequestMessageAsync(apiResource, body, parameters);
    }

    private async Task SendHttpRequestMessageAsync(ApiResource apiResource, string? body, params object[] parameters)
    {
        var baseUri = apiResource.EndpointFromResource(parameters);

        using var requestMessage = new HttpRequestMessage(apiResource.HttpMethod, baseUri);
        requestMessage.Content = body is not null
            ? new StringContent(body, Encoding.UTF8, "application/json")
            : null;

        using var responseMessage = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

        ResponseStatusCode = responseMessage.StatusCode;
        ResponseLocation = responseMessage.Headers.Location;
        ResponseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

        LogUnexpectedErrors(_testOutputHelper);
    }

    public static IHttpClientDriver CreateDriver(HttpClient httpClient, ITestOutputHelper testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        return new HttpClientDriver(httpClient, testOutputHelper);
    }

    private void LogUnexpectedErrors(ITestOutputHelper testOutputHelper)
    {
        if (ResponseStatusCode != HttpStatusCode.InternalServerError) return;

        var content = string.IsNullOrWhiteSpace(ResponseContent) ? "<empty response>" : ResponseContent;
        testOutputHelper.WriteLine($"HTTP 500 Response: {content}");
    }
}
