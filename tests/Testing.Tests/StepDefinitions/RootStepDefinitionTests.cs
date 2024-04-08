using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.StepDefinitions;

namespace DrifterApps.Seeds.Testing.Tests.StepDefinitions;

[UnitTest]
public class RootStepDefinitionTests
{
    private readonly RootStepDefinitionDriver _driver = new();

    [Fact]
    public async Task GivenWithCreatedId_WhenCreatedIdValid_ThenReturnCreatedId()
    {
        // Arrange
        var createdId = Guid.NewGuid();
        var sut = _driver.WithCreatedId(createdId).Build();

        // Act
        var result = await sut.WithCreatedIdAsync();

        // Assert
        result.Should().Be(createdId);
    }

    [Fact]
    public async Task GivenWithCreatedId_WhenCreatedIdEmpty_ThenReturnCreatedId()
    {
        // Arrange
        var sut = _driver.WithNoCreatedId().Build();

        // Act
        var action = () => sut.WithCreatedIdAsync();

        // Assert
        await action.Should().ThrowAsync<XunitException>();
    }

    [Fact]
    public async Task GivenWithResultAs_WhenCreatedIdEmpty_ThenReturnCreatedId()
    {
        // Arrange
        var sut = _driver.WithStringResultData().Build();

        // Act
        var result = await sut.WithResultAs<string>();

        // Assert
        result.Should().NotBeEmpty();
    }

    private sealed class RootStepDefinitionDriver : IDriverOf<MockStepDefinition>
    {
        private readonly IHttpClientDriver _httpClientDriver = Substitute.For<IHttpClientDriver>();

        public MockStepDefinition Build() => new(_httpClientDriver);

        public RootStepDefinitionDriver WithCreatedId(Guid id)
        {
            _httpClientDriver.DeserializeContentAsync<StepDefinition.Created>()
                .Returns(new StepDefinition.Created(id));

            return this;
        }

        public RootStepDefinitionDriver WithStringResultData()
        {
            _httpClientDriver.DeserializeContentAsync<string>().Returns(Fakerizer.Lorem.Text());

            return this;
        }

        public RootStepDefinitionDriver WithNoCreatedId()
        {
            _httpClientDriver
                .DeserializeContentAsync<StepDefinition.Created>()
                .Returns(_ => Task.FromResult(null as StepDefinition.Created));

            return this;
        }
    }

    private class MockStepDefinition(IHttpClientDriver httpClientDriver) : StepDefinition(httpClientDriver);
}
