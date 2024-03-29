using System.Text.Json;
using DrifterApps.Seeds.Infrastructure;
using DrifterApps.Seeds.Testing;
using MediatR;

namespace Infrastructure.Tests;

[UnitTest]
public class MediatorSerializedObjectTests
{
    private readonly Driver _driver = new();

    [Fact]
    public void GivenConstructor_WhenFullTypeNameNotFound_ThenShouldThrowArgumentException()
    {
        // Arrange
        _driver.GivenInvalidAssemblyQualifiedName();

        // Act
        Action action = () => _ = _driver.Build();

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Invalid Type name (Parameter '*')");
    }

    [Fact]
    public void GivenConstructor_WhenFullTypeNameIsNull_ThenShouldThrowArgumentException()
    {
        // Arrange
        _driver.GivenNullAssemblyQualifiedName();

        // Act
        Action action = () => _ = _driver.Build();

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Missing assembly name for '{_driver.Description}' (Parameter 'assemblyQualifiedName')");
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var mediatorSerializedObject = _driver.Build();

        // Act
        var result = mediatorSerializedObject.ToString();

        // Assert
        result.Should().Be($"SampleRequest {_driver.Description}");
    }

    [Fact]
    public void TryDeserializeObject_WithValidData_ShouldDeserializeObject()
    {
        // Arrange
        var mediatorSerializedObject = _driver.Build();

        // Act
        var success = mediatorSerializedObject.TryDeserializeObject(out var deserializedObject);

        // Assert
        success.Should().BeTrue();
        deserializedObject.Should().NotBeNull();
        deserializedObject.Should().BeOfType<SampleRequest>();
        var sampleRequest = (SampleRequest) deserializedObject!;
        sampleRequest.Should().BeEquivalentTo(_driver.OriginalRequest);
    }

    [Fact]
    public void TryDeserializeObject_WithInvalidData_ShouldReturnFalse()
    {
        // Arrange
        var mediatorSerializedObject = _driver.GivenInvalidData().Build();

        // Act
        var success = mediatorSerializedObject.TryDeserializeObject(out var deserializedObject);

        // Assert
        success.Should().BeFalse();
        deserializedObject.Should().BeNull();
    }

    [Fact]
    public void SerializeObject_ShouldReturnMediatorSerializedObject()
    {
        // Arrange
        var mediatorSerializedObject = _driver.Build();

        // Act
        var result = MediatorSerializedObject.SerializeObject(_driver.OriginalRequest, _driver.Description);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mediatorSerializedObject);
    }

    private class Driver : IDriverOf<MediatorSerializedObject>
    {
        private string _assemblyQualifiedName = typeof(SampleRequest).AssemblyQualifiedName!;

        private string _data;

        public Driver()
        {
            OriginalRequest = new Faker<SampleRequest>()
                .RuleFor(x => x.Property1, faker => faker.Random.Word())
                .RuleFor(x => x.Property2, faker => faker.Random.Int());
            Description = Fakerizer.Lorem.Sentence();
            _data = JsonSerializer.Serialize(OriginalRequest);
        }

        public string Description { get; }

        public SampleRequest OriginalRequest { get; }

        public MediatorSerializedObject Build() => new(_assemblyQualifiedName, _data, Description);

        public Driver GivenInvalidAssemblyQualifiedName()
        {
            _assemblyQualifiedName = Fakerizer.Lorem.Word();

            return this;
        }

        public Driver GivenNullAssemblyQualifiedName()
        {
            _assemblyQualifiedName = null!;

            return this;
        }

        public Driver GivenInvalidData()
        {
            _data = "{\"Property1\":\"Value1\",\"InvalidProperty\":42}";
            _assemblyQualifiedName = typeof(string).AssemblyQualifiedName!;

            return this;
        }
    }

    private class SampleRequest : IBaseRequest
    {
        public string Property1 { get; } = null!;
        public int Property2 { get; }
    }
}
