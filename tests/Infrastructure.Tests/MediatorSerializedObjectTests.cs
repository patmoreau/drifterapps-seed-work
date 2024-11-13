using System.Text.Json;
using DrifterApps.Seeds.Application.Converters;
using DrifterApps.Seeds.Domain;
using DrifterApps.Seeds.Infrastructure;
using DrifterApps.Seeds.Testing;
using MediatR;

namespace Infrastructure.Tests;

[UnitTest]
public class MediatorSerializedObjectTests
{
    private static readonly JsonSerializerOptionsFactory Factory = new(() =>
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new StronglyTypedIdJsonConverterFactory());
        return options;
    });

    private readonly Driver _driver = new(Factory);

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
        var options = Factory.CreateOptions();
        var mediatorSerializedObject = _driver.Build();

        // Act
        var success = mediatorSerializedObject.TryDeserializeObject(out var deserializedObject, options);

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
        var options = Factory.CreateOptions();
        var mediatorSerializedObject = _driver.GivenInvalidData().Build();

        // Act
        var success = mediatorSerializedObject.TryDeserializeObject(out var deserializedObject, options);

        // Assert
        success.Should().BeFalse();
        deserializedObject.Should().BeNull();
    }

    [Fact]
    public void SerializeObject_ShouldReturnMediatorSerializedObject()
    {
        // Arrange
        var options = Factory.CreateOptions();
        var mediatorSerializedObject = _driver.Build();

        // Act
        var result = MediatorSerializedObject.SerializeObject(_driver.OriginalRequest, _driver.Description, options);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mediatorSerializedObject);
    }

    private class Driver : IDriverOf<MediatorSerializedObject>
    {
        private readonly Faker<SampleRequest> _faker = new Faker<SampleRequest>()
            .RuleFor(x => x.Id, faker => (MyId) faker.Random.Guid())
            .RuleFor(x => x.Property1, faker => faker.Random.Word())
            .RuleFor(x => x.Property2, faker => faker.Random.Int());

        private string _assemblyQualifiedName = typeof(SampleRequest).AssemblyQualifiedName!;

        private string _data;

        public Driver(JsonSerializerOptionsFactory factory)
        {
            OriginalRequest = _faker.Generate();
            Description = Fakerizer.Lorem.Sentence();
            _data = JsonSerializer.Serialize(OriginalRequest, factory.CreateOptions());
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

#pragma warning disable CA1515
    public record MyId : StronglyTypedId<MyId>;
#pragma warning restore CA1515

    internal class SampleRequest : IBaseRequest
    {
        public required MyId Id { get; init; }
        public required string Property1 { get; init; }
        public int Property2 { get; init; }
    }
}
