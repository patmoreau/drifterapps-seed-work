using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DrifterApps.Seeds.Testing.Infrastructure;
using DrifterApps.Seeds.Testing.Infrastructure.Authentication;
using FluentAssertions;
using Xunit.Abstractions;

namespace DrifterApps.Seeds.Testing.Drivers;

public class HttpClientDriver : IHttpClientDriver
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _testOutputHelper;

    public HttpClientDriver(HttpClient httpClient, ITestOutputHelper testOutputHelper)
    {
        _httpClient = httpClient;
        _testOutputHelper = testOutputHelper;
    }

    public HttpResponseMessage? ResponseMessage { get; private set; }

    public void ShouldBeUnauthorized() =>
        ResponseMessage.Should().NotBeNull()
            .And.HaveStatusCode(HttpStatusCode.Unauthorized);

    public void ShouldHaveResponseWithStatus(HttpStatusCode httpStatus)
    {
        LogUnexpectedContent(httpStatus);

        ResponseMessage.Should()
            .NotBeNull()
            .And.Subject.StatusCode.Should().Be(httpStatus);
    }

    public void ShouldHaveResponseWithStatus(Func<HttpStatusCode?, bool> httpStatusPredicate)
    {
        if (httpStatusPredicate == null) throw new ArgumentNullException(nameof(httpStatusPredicate));

        ResponseMessage.Should().NotBeNull();
        httpStatusPredicate(ResponseMessage!.StatusCode).Should().BeTrue();
    }

    public void ShouldNotHaveResponseWithOneOfStatuses(params HttpStatusCode[] httpStatuses) =>
        ResponseMessage.Should()
            .NotBeNull()
            .And.Subject.StatusCode.Should().BeOneOf(httpStatuses);

    public T? DeserializeContent<T>()
    {
        var resultAsString = ResponseMessage?.Content.ReadAsStringAsync().Result;
        if (resultAsString is null) return default;

        var content = JsonSerializer.Deserialize<T>(resultAsString,
            new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
        return content;
    }

    public void AuthenticateUser(Guid userId) =>
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(MockAuthenticationHandler.AuthenticationScheme, userId.ToString());

    public void UnAuthenticate() => _httpClient.DefaultRequestHeaders.Authorization = null;

    public async Task SendGetRequest(ApiResource apiResource, string? query = null)
    {
        ArgumentNullException.ThrowIfNull(apiResource);

        var baseUri = apiResource.EndpointFromResource();
        var fullUri = baseUri;
        if (query is not null)
            fullUri = new Uri($"{fullUri}?{query}", fullUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);

        using HttpRequestMessage request = new(HttpMethod.Get, fullUri);
        await SendRequest(request).ConfigureAwait(false);
    }

    public async Task SendGetRequest(ApiResource apiResource, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(apiResource);

        var endpointUri = apiResource.EndpointFromResource(parameters);
        using HttpRequestMessage request = new(HttpMethod.Get, endpointUri);
        await SendRequest(request).ConfigureAwait(false);
    }

    public async Task SendPostRequest(ApiResource apiResource, string? body = null)
    {
        ArgumentNullException.ThrowIfNull(apiResource);

        var endpointUri = apiResource.EndpointFromResource();

        using HttpRequestMessage request = new(HttpMethod.Post, endpointUri);
        if (body is not null) request.Content = new StringContent(body, Encoding.UTF8, "application/json");

        await SendRequest(request).ConfigureAwait(false);
    }

    public async Task SendDeleteRequest(ApiResource apiResource, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(apiResource);

        var endpointUri = apiResource.EndpointFromResource(parameters);
        using HttpRequestMessage request = new(HttpMethod.Delete, endpointUri);
        await SendRequest(request).ConfigureAwait(false);
    }

    private async Task SendRequest(HttpRequestMessage requestMessage)
    {
        ResponseMessage = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

        LogUnexpectedErrors();
    }

    private void LogUnexpectedErrors()
    {
        if (ResponseMessage?.StatusCode != HttpStatusCode.InternalServerError) return;

        var resultAsString = ResponseMessage?.Content.ReadAsStringAsync().Result;

        _testOutputHelper.WriteLine($"HTTP 500 Response: {resultAsString ?? "<unknown>"}");
    }

    private void LogUnexpectedContent(HttpStatusCode expectedStatusCode)
    {
        if (ResponseMessage?.StatusCode == expectedStatusCode) return;

        var resultAsString = ResponseMessage?.Content.ReadAsStringAsync().Result;

        _testOutputHelper.WriteLine(
            $"Unexpected HTTP {ResponseMessage?.StatusCode} Code with Response: {resultAsString ?? "<unknown>"}");
    }
}
