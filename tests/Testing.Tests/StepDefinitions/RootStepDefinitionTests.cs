using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.StepDefinitions;

namespace DrifterApps.Seeds.Testing.Tests.StepDefinitions;

[UnitTest]
public class RootStepDefinitionTests
{
    private readonly RootStepDefinitionDriver _driver = new();

    [Fact]
    public void GivenWithCreatedId_WhenCreatedIdValid_ThenReturnCreatedId()
    {
        // Arrange
        var createdId = Guid.NewGuid();
        var sut = _driver.WithCreatedId(createdId).Build();

        // Act
        var result = sut.WithCreatedId();

        // Assert
        result.Should().Be(createdId);
    }

    [Fact]
    public void GivenWithCreatedId_WhenCreatedIdEmpty_ThenReturnCreatedId()
    {
        // Arrange
        var sut = _driver.WithNoCreatedId().Build();

        // Act
        Action action = () => sut.WithCreatedId();

        // Assert
        action.Should().Throw<XunitException>();
    }

    [Fact]
    public void GivenWithResultAs_WhenCreatedIdEmpty_ThenReturnCreatedId()
    {
        // Arrange
        var sut = _driver.WithStringResultData().Build();

        // Act
        var result = sut.WithResultAs<string>();

        // Assert
        result.Should().NotBeEmpty();
    }

    private sealed class RootStepDefinitionDriver : IDriverOf<MockStepDefinition>
    {
        private readonly IHttpClientDriver _httpClientDriver = Substitute.For<IHttpClientDriver>();

        public MockStepDefinition Build() => new(_httpClientDriver);

        public RootStepDefinitionDriver WithCreatedId(Guid id)
        {
            _httpClientDriver.DeserializeContent<StepDefinition.Created>()
                .Returns(new StepDefinition.Created(id));

            return this;
        }

        public RootStepDefinitionDriver WithStringResultData()
        {
            _httpClientDriver.DeserializeContent<string>().Returns(Fakerizer.Lorem.Text());

            return this;
        }

        public RootStepDefinitionDriver WithNoCreatedId()
        {
            _httpClientDriver.DeserializeContent<StepDefinition.Created>().Returns(_ => null);

            return this;
        }
    }

    public class MockStepDefinition : StepDefinition
    {
        public MockStepDefinition(IHttpClientDriver httpClientDriver) : base(httpClientDriver)
        {
        }
    }
}
