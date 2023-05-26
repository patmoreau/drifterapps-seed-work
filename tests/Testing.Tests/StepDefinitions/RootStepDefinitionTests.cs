using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.StepDefinitions;
using FluentAssertions;
using NSubstitute;
using Xunit.Sdk;

namespace DrifterApps.Seeds.Testing.Tests.StepDefinitions;

public class RootStepDefinitionTests
{
    private readonly RootStepDefinitionDriver _driver = new();

    [Fact]
#pragma warning disable CA1707
    public void GivenWithCreatedId_WhenCreatedIdValid_ThenReturnCreatedId()
#pragma warning restore CA1707
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
#pragma warning disable CA1707
    public void GivenWithCreatedId_WhenCreatedIdEmpty_ThenReturnCreatedId()
#pragma warning restore CA1707
    {
        // Arrange
        var sut = _driver.WithNoCreatedId().Build();

        // Act
        Action action = () => sut.WithCreatedId();

        // Assert
        action.Should().Throw<XunitException>();
    }

    private sealed class RootStepDefinitionDriver : IDriverOf<RootStepDefinition>
    {
        private readonly HttpClientDriver _httpClientDriver = Substitute.For<HttpClientDriver>();

        public RootStepDefinition Build() => new MockRootStepDefinition(_httpClientDriver);

        public RootStepDefinitionDriver WithCreatedId(Guid id)
        {
            _httpClientDriver.DeserializeContent<Created>().Returns(new Created(id));

            return this;
        }

        public RootStepDefinitionDriver WithNoCreatedId()
        {
            _httpClientDriver.DeserializeContent<Created>().Returns(_ => null);

            return this;
        }
    }
#pragma warning disable CA1034
    public class MockRootStepDefinition : RootStepDefinition
#pragma warning restore CA1034
    {
        public MockRootStepDefinition(HttpClientDriver httpClientDriver) : base(httpClientDriver)
        {
        }
    }

    private sealed record Created(Guid Id);
}
