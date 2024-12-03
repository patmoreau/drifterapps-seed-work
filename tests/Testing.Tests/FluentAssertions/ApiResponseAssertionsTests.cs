using System.Globalization;
using System.Net;
using System.Text;
using Bogus;
using DrifterApps.Seeds.Testing.Tests.Drivers;
using Xunit.Sdk;

namespace DrifterApps.Seeds.Testing.Tests.FluentAssertions;

[UnitTest]
public class ApiResponseAssertionsTests(ApiResponseDriver driver) : IClassFixture<ApiResponseDriver>
{
    private readonly Faker _faker = new();

    private IApiResponseWireMock Api => driver.Api.Value;

    [Fact]
    public async Task GivenMonitoringMessage_WhenContainsCorrelationId_ShouldBeIncludedInMessage()
    {
        // Arrange
        var response = await Api.GetWithCorrelationIdAsync(driver.CorrelationId);
        var expectedMessage = new StringBuilder("Expected response to be successful, but it was not.")
            .Append('*')
            .Append("Request")
            .Append('*')
            .Append("RequestUri")
            .Append('*')
            .Append(CultureInfo.InvariantCulture, $"XCorrelationId = \"{driver.CorrelationId}\"")
            .Append('*')
            .Append("Response")
            .Append('*')
            .Append(CultureInfo.InvariantCulture, $"XCorrelationId = \"{driver.CorrelationId}\"")
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
        var response = await Api.GetWithoutCorrelationIdAsync();
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
        var response = await Api.GetIsSuccessfulAsync();

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
        var response = await Api.GetIsNotSuccessfulAsync();
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
        var response = await Api.GetIsFailureAsync();

        // Act
        Action act = () => response.Should().BeFailure();

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [ClassData(typeof(BecauseData))]
    public async Task GivenBeFailure_WhenResponseIsNotFailure_ShouldThrow(string because, object[] becauseArgs)
    {
        // Arrange
        var response = await Api.GetIsNotFailureAsync();
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
        var response = await Api.GetIsAuthorizedAsync();

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
        var response = await Api.GetIsNotAuthorizedAsync();
        var expectedMessage = GetExpectedMessage(
            $"Expected response to be authorized{{0}}, but {FormatHttpCodeMessage(driver.NotAuthorizedStatusCode)} was not.*",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().BeAuthorized(because, becauseArgs);

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenBeForbidden_WhenResponseIsForbidden_ShouldNotThrow()
    {
        // Arrange
        var response = await Api.GetIsForbiddenAsync();

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
        var response = await Api.GetIsNotForbiddenAsync();
        var expectedMessage = GetExpectedMessage(
            $"Expected response to be forbidden{{0}}, but {FormatHttpCodeMessage(driver.NotForbiddenStatusCode)} was not.*",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().BeForbidden(because, becauseArgs);

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenNotBeAuthorized_WhenResponseIsUnauthorized_ShouldNotThrow()
    {
        // Arrange
        var response = await Api.GetIsUnauthorizedAsync();

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
        var response = await Api.GetIsNotUnauthorizedAsync();
        var expectedMessage = GetExpectedMessage(
            $"Expected response not to be authorized{{0}}, but it was {FormatHttpCodeMessage(driver.SuccessStatusCode)}.*",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().NotBeAuthorized(because, becauseArgs);

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenHaveStatusCode_WhenResponseHave_ShouldNotThrow()
    {
        // Arrange
        var response = await Api.GetIsWithStatusCodeAsync();

        // Act
        Action act = () => response.Should().HaveStatusCode(driver.StatusCode);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [ClassData(typeof(BecauseData))]
    public async Task GivenHaveStatusCode_WhenResponseHaveNot_ShouldThrow(string because, object[] becauseArgs)
    {
        // Arrange
        var response = await Api.GetIsWithStatusCodeAsync();
        var expectedMessage = GetExpectedMessage(
            $"Expected response to have status code {FormatHttpCodeMessage(driver.NotStatusCode)}{{0}}, but it was {FormatHttpCodeMessage(driver.StatusCode)}.*",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().HaveStatusCode(driver.NotStatusCode, because, becauseArgs);

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenNotHaveStatusCode_WhenResponseNotHave_ShouldNotThrow()
    {
        // Arrange
        var response = await Api.GetIsNotWithStatusCodeAsync();

        // Act
        Action act = () => response.Should().NotHaveStatusCode(driver.StatusCode);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [ClassData(typeof(BecauseData))]
    public async Task GivenNotHaveStatusCode_WhenResponseHave_ShouldThrow(string because, object[] becauseArgs)
    {
        // Arrange
        var response = await Api.GetIsNotWithStatusCodeAsync();
        var expectedMessage = GetExpectedMessage(
            $"Expected response to not have status code {FormatHttpCodeMessage(driver.NotStatusCode)}{{0}}, but it was.*",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().NotHaveStatusCode(driver.NotStatusCode, because, becauseArgs);

        // Assert
        act.Should().Throw<XunitException>().WithMessage(expectedMessage);
    }

    [Fact]
    public async Task GivenWithError_WhenResponseHaveTheError_ShouldNotThrow()
    {
        // Arrange
        var response = await Api.GetIsWithErrorAsync();

        // Act
        Action act = () => response.Should().WithError(driver.ErrorMessage);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [ClassData(typeof(BecauseData))]
    public async Task GivenWithError_WhenResponseHaveNot_ShouldThrow(string because, object[] becauseArgs)
    {
        // Arrange
        var error = _faker.Lorem.Sentence();
        var response = await Api.GetIsWithErrorAsync();
        var expectedMessage = GetExpectedMessage(
            $"Expected response to have error \"{error}\"{{0}}, but found *",
            because, becauseArgs);

        // Act
        Action act = () => response.Should().WithError(error, because, becauseArgs);

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
            (int) statusCode);

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
}