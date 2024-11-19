using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using Bogus;
using Refit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit.Sdk;
using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace DrifterApps.Seeds.Testing.Tests.FluentAssertions;

[UnitTest]
public class ApiResponseAssertionsTests : IAsyncLifetime
{
    private readonly Driver _driver = new();
    private readonly Faker _faker = new();

    public Task InitializeAsync()
    {
        _driver.StartServer();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _driver.StopServer();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GivenMonitoringMessage_WhenContainsCorrelationId_ShouldBeIncludedInMessage()
    {
        // Arrange
        var api = _driver.WithCorrelationIdInResponse().Build();
        var response = await api.GetWithMonitoringHeadersAsync(_driver.CorrelationId);
        var expectedMessage = new StringBuilder("Expected response to be successful, but it was not.")
            .Append('*')
            .Append("Request")
            .Append('*')
            .Append("RequestUri")
            .Append('*')
            .Append(CultureInfo.InvariantCulture, $"XCorrelationId = \"{_driver.CorrelationId}\"")
            .Append('*')
            .Append("Response")
            .Append('*')
            .Append(CultureInfo.InvariantCulture, $"XCorrelationId = \"{_driver.CorrelationId}\"")
            .Append('*');

        // Act
        Action act = () => response.Should().BeSuccessful();

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage.ToString());
    }

    [Fact]
    public async Task GivenMonitoringMessage_WhenDoesNotContainCorrelationId_ShouldBeIncludedInMessage()
    {
        // Arrange
        var api = _driver.WithoutCorrelationIdInResponse().Build();
        var response = await api.GetAsync();
        var expectedMessage = "Expected response to be successful, but it was not.";

        // Act
        Action act = () => response.Should().BeSuccessful();

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenBeSuccessful_WhenResponseIsSuccessful_ShouldNotThrow()
    {
        // Arrange
        var api = _driver.IsSuccessful().Build();
        var response = await api.GetAsync();

        // Act
        Action act = () => response.Should().BeSuccessful();

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [ClassData(typeof(BecauseData))]
    public async Task GivenBeSuccessful_WhenResponseIsNotSuccessful_ShouldThrow(string because, object[] becauseArgs)
    {
        // Arrange
        var api = _driver.IsNotSuccessful().Build();
        var response = await api.GetAsync();
        var expectedMessage = GetExpectedMessage("Expected response to be successful{0}, but it was not.*",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().BeSuccessful(because, becauseArgs);

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenBeFailure_WhenResponseIsFailure_ShouldNotThrow()
    {
        // Arrange
        var api = _driver.IsNotSuccessful().Build();
        var response = await api.GetAsync();

        // Act
        Action act = () => response.Should().BeFailure();

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [ClassData(typeof(BecauseData))]
    public async Task GivenBeFailure_WhenResponseIsSuccessful_ShouldThrow(string because, object[] becauseArgs)
    {
        // Arrange
        var api = _driver.IsSuccessful().Build();
        var response = await api.GetAsync();
        var expectedMessage = GetExpectedMessage("Expected response to be a failure{0}, but it was not.",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().BeFailure(because, becauseArgs);

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenBeAuthorized_WhenResponseNotForbiddenNorUnauthorized_ShouldNotThrow()
    {
        // Arrange
        var statusCode = _faker.PickRandom(Enum.GetValues<HttpStatusCode>()
            .Where(x => x is not HttpStatusCode.Forbidden and not HttpStatusCode.Unauthorized));
        var api = _driver.IsWithStatusCode(statusCode).Build();
        var response = await api.GetAsync();

        // Act
        Action act = () => response.Should().BeAuthorized();

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [ClassData(typeof(BecauseData))]
    public async Task GivenBeAuthorized_WhenResponseIsForbiddenOrUnauthorized_ShouldThrow(string because,
        object[] becauseArgs)
    {
        // Arrange
        var expectedStatusCode = _faker.PickRandom(HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);
        var api = _driver.IsWithStatusCode(expectedStatusCode).Build();
        var response = await api.GetAsync();
        var expectedMessage = GetExpectedMessage(
            $"Expected response to be authorized{{0}}, but {FormatHttpCodeMessage(expectedStatusCode)} was not.*",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().BeAuthorized();

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenBeForbidden_WhenResponseForbidden_ShouldNotThrow()
    {
        // Arrange
        var statusCode = HttpStatusCode.Forbidden;
        var api = _driver.IsWithStatusCode(statusCode).Build();
        var response = await api.GetAsync();

        // Act
        Action act = () => response.Should().BeForbidden();

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [ClassData(typeof(BecauseData))]
    public async Task GivenBeForbidden_WhenResponseIsNotForbidden_ShouldThrow(string because, object[] becauseArgs)
    {
        // Arrange
        var expectedStatusCode = _faker.PickRandom(Enum.GetValues<HttpStatusCode>()
            .Where(x => x is not HttpStatusCode.Forbidden));
        var api = _driver.IsWithStatusCode(expectedStatusCode).Build();
        var response = await api.GetAsync();
        var expectedMessage = GetExpectedMessage(
            $"Expected response to be forbidden{{0}}, but {FormatHttpCodeMessage(expectedStatusCode)} was not.*",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().BeForbidden();

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenNotBeAuthorized_WhenResponseUnauthorized_ShouldNotThrow()
    {
        // Arrange
        var statusCode = HttpStatusCode.Unauthorized;
        var api = _driver.IsWithStatusCode(statusCode).Build();
        var response = await api.GetAsync();

        // Act
        Action act = () => response.Should().NotBeAuthorized();

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [ClassData(typeof(BecauseData))]
    public async Task GivenNotBeAuthorized_WhenResponseIsNotUnauthorized_ShouldThrow(string because,
        object[] becauseArgs)
    {
        // Arrange
        var statusCode = _faker.PickRandom(Enum.GetValues<HttpStatusCode>()
            .Where(x => x is not HttpStatusCode.Unauthorized));
        var api = _driver.IsWithStatusCode(statusCode).Build();
        var response = await api.GetAsync();
        var expectedMessage = GetExpectedMessage(
            $"Expected response not to be authorized{{0}}, but it was {FormatHttpCodeMessage(statusCode)}.*",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().NotBeAuthorized();

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenHaveStatusCode_WhenResponseHave_ShouldNotThrow()
    {
        // Arrange
        var expectedStatusCode = _faker.PickRandom<HttpStatusCode>();
        var api = _driver.IsWithStatusCode(expectedStatusCode).Build();
        var response = await api.GetAsync();

        // Act
        Action act = () => response.Should().HaveStatusCode(expectedStatusCode);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [ClassData(typeof(BecauseData))]
    public async Task GivenHaveStatusCode_WhenResponseHaveNot_ShouldThrow(string because, object[] becauseArgs)
    {
        // Arrange
        var expectedStatusCode = _faker.PickRandom<HttpStatusCode>();
        var statusCode = _faker.PickRandom(Enum.GetValues<HttpStatusCode>().Where(x => x != expectedStatusCode));
        var api = _driver.IsWithStatusCode(statusCode).Build();
        var response = await api.GetAsync();
        var expectedMessage = GetExpectedMessage(
            $"Expected response to have status code {FormatHttpCodeMessage(expectedStatusCode)}{{0}}, but it was {FormatHttpCodeMessage(statusCode)}.*",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().HaveStatusCode(expectedStatusCode, because, becauseArgs);

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenNotHaveStatusCode_WhenResponseNotHave_ShouldNotThrow()
    {
        // Arrange
        var expectedStatusCode = _faker.PickRandom<HttpStatusCode>();
        var statusCode = _faker.PickRandom(Enum.GetValues<HttpStatusCode>().Where(x => x != expectedStatusCode));
        var api = _driver.IsWithStatusCode(statusCode).Build();
        var response = await api.GetAsync();

        // Act
        Action act = () => response.Should().NotHaveStatusCode(expectedStatusCode);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [ClassData(typeof(BecauseData))]
    public async Task GivenNotHaveStatusCode_WhenResponseHave_ShouldThrow(string because, object[] becauseArgs)
    {
        // Arrange
        var expectedStatusCode = _faker.PickRandom<HttpStatusCode>();
        var api = _driver.IsWithStatusCode(expectedStatusCode).Build();
        var response = await api.GetAsync();

        var expectedMessage = GetExpectedMessage(
            $"Expected response to not have status code {FormatHttpCodeMessage(expectedStatusCode)}{{0}}, but it was.*",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().NotHaveStatusCode(expectedStatusCode, because, becauseArgs);

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenWithError_WhenResponseHaveTheError_ShouldNotThrow()
    {
        // Arrange
        var expectedError = _faker.Lorem.Sentence();
        var api = _driver.IsWithError(expectedError).Build();
        var response = await api.GetAsync();

        // Act
        Action act = () => response.Should().WithError(expectedError);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [ClassData(typeof(BecauseData))]
    public async Task GivenWithError_WhenResponseHaveNot_ShouldThrow(string because, object[] becauseArgs)
    {
        // Arrange
        var expectedError = _faker.Lorem.Sentence();
        var error = _faker.Lorem.Sentence();
        var api = _driver.IsWithError(error).Build();
        var response = await api.GetAsync();
        var expectedMessage = GetExpectedMessage(
            $"Expected response to have error \"{expectedError}\"{{0}}, but found *",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().WithError(expectedError, because, becauseArgs);

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    private static string GetExpectedMessage(string messageFormat, string because, params object[] becauseArgs)
    {
        var expectedBecause = string.Format(CultureInfo.InvariantCulture, because, becauseArgs);
        return string.Format(CultureInfo.InvariantCulture, messageFormat,
            string.IsNullOrWhiteSpace(expectedBecause) ? "" : $" because {expectedBecause}");
    }

    private static string FormatHttpCodeMessage(HttpStatusCode statusCode) =>
        string.Format(CultureInfo.InvariantCulture, "HttpStatusCode.{0} {{{{value: {1}}}}}",
            Enum.GetName(statusCode) ?? "Unknown",
            (int)statusCode);

    internal class BecauseData : TheoryData<string, string[]>
    {
        public BecauseData()
        {
            Add("", []);
            var faker = new Faker();
            var because = faker.Lorem.Sentence();
            Add(because, faker.Lorem.Words(2));
            Add(because + "'{0};{1}'", faker.Lorem.Words(2));
        }
    }

    private class Driver : IDriverOf<IWireMock>
    {
        private readonly Faker _faker = new();
        private WireMockServer? _server;

        public Driver() => CorrelationId = _faker.Random.Guid();

        public Guid CorrelationId { get; }

        public IWireMock Build() => RestService.For<IWireMock>($"{_server?.Urls[0]}");

        public void StartServer() => _server = WireMockServer.Start();

        public void StopServer() => _server?.Stop();

        public Driver WithoutCorrelationIdInResponse()
        {
            _server?
                .Given(Request.Create().UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.BadRequest)
                );
            return this;
        }

        public Driver WithCorrelationIdInResponse()
        {
            _server?
                .Given(Request.Create().UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.BadRequest)
                        .WithHeader("X-Correlation-Id", CorrelationId.ToString())
                );
            return this;
        }

        public Driver IsNotSuccessful()
        {
            var responseCode = _faker.PickRandom(Enum.GetValues<HttpStatusCode>().Where(x => (int)x is >= 300));
            _server?
                .Given(Request.Create().UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(responseCode)
                );
            return this;
        }

        public Driver IsSuccessful()
        {
            var responseCode =
                _faker.PickRandom(Enum.GetValues<HttpStatusCode>().Where(x => (int)x is >= 200 and < 300));
            _server?
                .Given(Request.Create().UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(responseCode)
                );
            return this;
        }

        public Driver IsWithStatusCode(HttpStatusCode statusCode)
        {
            _server?
                .Given(Request.Create().UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(statusCode)
                );
            return this;
        }

        public Driver IsWithError(string message)
        {
            _server?
                .Given(Request.Create().UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.BadRequest)
                        .WithBody(JsonSerializer.Serialize(new ProblemDetails
                        {
                            Detail = message
                        }))
                );
            return this;
        }
    }
}
