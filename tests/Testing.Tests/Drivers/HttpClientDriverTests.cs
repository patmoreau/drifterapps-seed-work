using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Bogus;
using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Infrastructure;
using DrifterApps.Seeds.Testing.Infrastructure.Authentication;
using DrifterApps.Seeds.Testing.Tests.Builders;
using Xunit.Abstractions;

namespace DrifterApps.Seeds.Testing.Tests.Drivers;

[UnitTest]
public sealed class HttpClientDriverTests : IDisposable, IAsyncDisposable
{
    private static readonly Faker Faker = new();

    private readonly ApiResourceBuilder _apiResourceBuilder =
        FakerBuilder<ApiResource>.CreateBuilder<ApiResourceBuilder>();

    private readonly Driver _driver = new();

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GivenDeserializeContent_WhenResponseMessageIsNull_ThenReturnsDefault()
    {
        // Arrange
        var apiResource = _apiResourceBuilder.Build();
        var sut = _driver.WithEmptyResponseMessage().Build();

        // Act
        await sut.SendRequestAsync(apiResource);
        var result = sut.DeserializeContent<object>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GivenDeserializeContent_WhenResponseMessageContainsData_ThenReturnsData()
    {
        // Arrange
        var data = new TestObject(Guid.NewGuid());
        var apiResource = _apiResourceBuilder.Build();
        var sut = _driver.WithResponseMessage(data).Build();

        // Act
        await sut.SendRequestAsync(apiResource);
        var result = sut.DeserializeContent<TestObject>();

        // Assert
        result.Should().NotBeNull().And.BeEquivalentTo(data);
    }

    [Fact]
    public void GivenAuthenticateUser_WhenInvoked_ThenAuthorizationSet()
    {
        // Arrange
        var userId = Faker.Internet.UserName();
        var sut = _driver.Build();

        // Act
        sut.AuthenticateUser(userId);

        // Assert
        _driver.ShouldHaveAuthenticatedUser(userId);
    }

    [Fact]
    public void GivenUnAuthenticate_WhenInvoked_ThenAuthorizationIsNull()
    {
        // Arrange
        var userId = Faker.Internet.UserName();
        var sut = _driver.Build();
        sut.AuthenticateUser(userId);

        // Act
        sut.UnAuthenticate();

        // Assert
        _driver.ShouldHaveNoAuthenticatedUser();
    }

    [Fact]
    public async Task GivenSendRequestWithBodyAsync_WhenApiResourceIsNull_ThenThrowsArgumentNullException()
    {
        // Arrange
        var sut = _driver.Build();

        // Act
        // ReSharper disable once AccessToDisposedClosure
        var act = async () => await sut.SendRequestWithBodyAsync(null!, Faker.Random.Words());

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GivenSendRequestWithBodyAsync_WhenBodyIsNullOfEmpty_ThenThrowsArgumentException(string? body)
    {
        // Arrange
        var apiResource = _apiResourceBuilder.Build();
        var sut = _driver.Build();

        // Act
        // ReSharper disable once AccessToDisposedClosure
        var act = async () => await sut.SendRequestWithBodyAsync(apiResource, body!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [MemberData(nameof(SendRequestWithBodyAsyncTestData))]
    public async Task GivenSendRequestWithBodyAsync_WhenHttpMethod_ThenSendGetHttpRequestMessage(HttpMethod httpMethod,
        string endpointTemplate, string body)
    {
        // Arrange
        var apiResource = _apiResourceBuilder
            .WithHttpMethod(httpMethod)
            .WithEndpointTemplate(endpointTemplate)
            .Build();
        var sut = _driver.WithSuccessfulResponseMessage().Build();

        // Act
        await sut.SendRequestWithBodyAsync(apiResource, body);

        // Assert
        _driver.ShouldHaveSentHttpRequestMessageWithContent(apiResource.HttpMethod, apiResource.EndpointTemplate);
    }

    [Fact]
    public async Task GivenSendRequestAsync_WhenApiResourceIsNull_ThenThrowsArgumentNullException()
    {
        // Arrange
        var sut = _driver.Build();

        // Act
        // ReSharper disable once AccessToDisposedClosure
        var act = async () => await sut.SendRequestAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [MemberData(nameof(SendRequestAsyncTestData))]
    public async Task GivenSendRequestAsync_WhenHttpMethod_ThenSendGetHttpRequestMessage(HttpMethod httpMethod,
        string endpointTemplate)
    {
        // Arrange
        var apiResource = _apiResourceBuilder
            .WithHttpMethod(httpMethod)
            .WithEndpointTemplate(endpointTemplate)
            .Build();
        var sut = _driver.WithSuccessfulResponseMessage().Build();

        // Act
        await sut.SendRequestAsync(apiResource);

        // Assert
        _driver.ShouldHaveSentHttpRequestMessage(apiResource.HttpMethod, apiResource.EndpointTemplate);
    }

    [Fact]
    public void GivenCreateDriver_WhenHttpClientIsNull_ThenThrowsArgumentNullException()
    {
        // Arrange
        var testOutputHelper = Substitute.For<ITestOutputHelper>();

        // Act
        var act = () => HttpClientDriver.CreateDriver(null!, testOutputHelper);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _driver.Dispose();
        }
    }

    private ValueTask DisposeAsyncCore()
    {
        _driver.Dispose();
        return ValueTask.CompletedTask;
    }

    ~HttpClientDriverTests() => Dispose(false);

    internal sealed class Driver : IDriverOf<HttpClientDriver>, IDisposable
    {
        private readonly IHttpMessageHandler _httpMessageHandler = Substitute.For<IHttpMessageHandler>();
        private readonly ITestOutputHelper _testOutputHelper = Substitute.For<ITestOutputHelper>();
        private HttpClient? _httpClient;
        private MockHttpMessageHandler? _mockHttpMessageHandler;
        private HttpResponseMessage? _responseMessage;

        public void Dispose() => Dispose(true);

        public HttpClientDriver Build()
        {
            _mockHttpMessageHandler = new MockHttpMessageHandler(_httpMessageHandler);
            _httpClient = new HttpClient(_mockHttpMessageHandler)
            {
                BaseAddress = new Uri("http://localhost")
            };
            return (HttpClientDriver) HttpClientDriver.CreateDriver(_httpClient, _testOutputHelper);
        }

        public Driver WithEmptyResponseMessage()
        {
            _responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            _httpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
                .Returns(_responseMessage);

            return this;
        }

        public Driver WithResponseMessage(object data, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _responseMessage = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(JsonSerializer.Serialize(data))
            };

            _httpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
                .Returns(_responseMessage);

            return this;
        }

        public Driver WithSuccessfulResponseMessage()
        {
            _responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(Faker.Random.Words()))
            };

            _httpMessageHandler.MockSendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
                .Returns(_responseMessage);

            return this;
        }

        public void ShouldHaveAuthenticatedUser(string userId) =>
            _httpClient!.DefaultRequestHeaders.Authorization.Should().NotBeNull().And.BeEquivalentTo(
                new AuthenticationHeaderValue(MockAuthenticationHandler.AuthenticationScheme, userId));

        public void ShouldHaveNoAuthenticatedUser() =>
            _httpClient!.DefaultRequestHeaders.Authorization.Should().BeNull();

        public void ShouldHaveSentHttpRequestMessage(HttpMethod httpMethod, string endpoint)
        {
            var uri = new Uri(_httpClient?.BaseAddress!, endpoint);
            _httpMessageHandler.Received(1).MockSendAsync(
                Arg.Is<HttpRequestMessage>(x => x.Method.Equals(httpMethod) && x.RequestUri!.Equals(uri)),
                Arg.Any<CancellationToken>());
        }

        public void ShouldHaveSentHttpRequestMessageWithContent(HttpMethod httpMethod, string endpoint)
        {
            var uri = new Uri(_httpClient?.BaseAddress!, endpoint);
            _httpMessageHandler.Received(1).MockSendAsync(
                Arg.Is<HttpRequestMessage>(x => x.Method.Equals(httpMethod)
                                                && x.RequestUri!.Equals(uri)
                                                && x.Content != null),
                Arg.Any<CancellationToken>());
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _mockHttpMessageHandler?.Dispose();
            _responseMessage?.Dispose();
            _httpClient?.Dispose();
        }
    }

    private record TestObject(Guid Id);

#pragma warning disable CA2211
    public static TheoryData<HttpMethod, string, string> SendRequestWithBodyAsyncTestData = new()
    {
        {HttpMethod.Get, Faker.Internet.UrlRootedPath(), JsonSerializer.Serialize(Faker.Address)},
        {HttpMethod.Post, Faker.Internet.UrlRootedPath(), JsonSerializer.Serialize(Faker.Address)},
        {HttpMethod.Put, Faker.Internet.UrlRootedPath(), JsonSerializer.Serialize(Faker.Address)},
        {HttpMethod.Patch, Faker.Internet.UrlRootedPath(), JsonSerializer.Serialize(Faker.Address)},
        {HttpMethod.Delete, Faker.Internet.UrlRootedPath(), JsonSerializer.Serialize(Faker.Address)}
    };

    public static TheoryData<HttpMethod, string> SendRequestAsyncTestData = new()
    {
        {HttpMethod.Get, Faker.Internet.UrlRootedPath()},
        {HttpMethod.Post, Faker.Internet.UrlRootedPath()},
        {HttpMethod.Put, Faker.Internet.UrlRootedPath()},
        {HttpMethod.Patch, Faker.Internet.UrlRootedPath()},
        {HttpMethod.Delete, Faker.Internet.UrlRootedPath()}
    };
#pragma warning restore CA2211
}
