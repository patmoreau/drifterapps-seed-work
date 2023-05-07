using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DrifterApps.Seeds.Tests.Infrastructure;
using DrifterApps.Seeds.Tests.Infrastructure.Authentication;
using FluentAssertions;
using Xunit.Abstractions;

namespace DrifterApps.Seeds.Tests.Drivers;

public class HttpClientDriver
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _testOutputHelper;

    public HttpClientDriver(HttpClient httpClient, ITestOutputHelper testOutputHelper)
    {
        _httpClient = httpClient;
        _testOutputHelper = testOutputHelper;
    }

    public HttpResponseMessage? ResponseMessage { get; private set; }

    private async Task SendRequest(HttpRequestMessage requestMessage)
    {
        ResponseMessage = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

        LogUnexpectedErrors();
    }

    internal async Task SendGetRequest(ApiResource apiResources, string? query = null)
    {
        Uri baseUri = apiResources.EndpointFromResource();
        Uri fullUri = baseUri;
        if (query is not null)
        {
            fullUri = new Uri($"{fullUri}?{query}", fullUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }

        using HttpRequestMessage request = new(HttpMethod.Get, fullUri);
        await SendRequest(request).ConfigureAwait(false);
    }

    internal async Task SendGetRequest(ApiResource apiResources, params object[] parameters)
    {
        Uri endpointUri = apiResources.EndpointFromResource(parameters);
        using HttpRequestMessage request = new(HttpMethod.Get, endpointUri);
        await SendRequest(request).ConfigureAwait(false);
    }

    internal async Task SendPostRequest(ApiResource apiResources, string? body = null)
    {
        Uri endpointUri = apiResources.EndpointFromResource();

        using HttpRequestMessage request = new(HttpMethod.Post, endpointUri);
        if (body is not null)
        {
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        await SendRequest(request).ConfigureAwait(false);
    }

    internal async Task SendDeleteRequest(ApiResource apiResources, params object[] parameters)
    {
        Uri endpointUri = apiResources.EndpointFromResource(parameters);
        using HttpRequestMessage request = new(HttpMethod.Delete, endpointUri);
        await SendRequest(request).ConfigureAwait(false);
    }

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

    public void ShouldNotHaveResponseWithOneOfStatuses(params HttpStatusCode[] httpStatuses) =>
        ResponseMessage.Should()
            .NotBeNull()
            .And.Subject.StatusCode.Should().BeOneOf(httpStatuses);

    public void ShouldHaveResponseWithStatus(Func<HttpStatusCode?, bool> httpStatusPredicate)
    {
        if (httpStatusPredicate == null)
        {
            throw new ArgumentNullException(nameof(httpStatusPredicate));
        }

        ResponseMessage.Should().NotBeNull();
        httpStatusPredicate(ResponseMessage!.StatusCode).Should().BeTrue();
    }

    public T? DeserializeContent<T>()
    {
        var resultAsString = ResponseMessage?.Content.ReadAsStringAsync().Result;
        if (resultAsString is null)
        {
            return default;
        }

        T? content = JsonSerializer.Deserialize<T>(resultAsString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return content;
    }

    public void AuthenticateUser(Guid userId) =>
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(MockAuthenticationHandler.AuthenticationScheme, userId.ToString());

    public void UnAuthenticate() => _httpClient.DefaultRequestHeaders.Authorization = null;

    private void LogUnexpectedErrors()
    {
        if (ResponseMessage?.StatusCode != HttpStatusCode.InternalServerError)
        {
            return;
        }

        var resultAsString = ResponseMessage?.Content.ReadAsStringAsync().Result;

        _testOutputHelper.WriteLine($"HTTP 500 Response: {resultAsString ?? "<unknown>"}");
    }

    private void LogUnexpectedContent(HttpStatusCode expectedStatusCode)
    {
        if (ResponseMessage?.StatusCode == expectedStatusCode)
        {
            return;
        }

        var resultAsString = ResponseMessage?.Content.ReadAsStringAsync().Result;

        _testOutputHelper.WriteLine(
            $"Unexpected HTTP {ResponseMessage?.StatusCode} Code with Response: {resultAsString ?? "<unknown>"}");
    }
}
