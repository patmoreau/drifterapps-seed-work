using MediatR;

namespace DrifterApps.Seeds.Infrastructure.Tests;

public class MediatorSerializedObjectTests
{
    private readonly static string SampleRequestTypeName = typeof(SampleRequest).AssemblyQualifiedName!;

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var fullTypeName = SampleRequestTypeName;
        var data = "{\"Property1\":\"Value1\",\"Property2\":42}";
        var additionalDescription = "Sample Description";
        var mediatorSerializedObject = new MediatorSerializedObject(fullTypeName, data, additionalDescription);

        // Act
        var result = mediatorSerializedObject.ToString();

        // Assert
        result.Should().Be("MediatorSerializedObjectTests+SampleRequest Sample Description");
    }

    [Fact]
    public void TryDeserializeObject_WithValidData_ShouldDeserializeObject()
    {
        // Arrange
        var fullTypeName = SampleRequestTypeName;
        var data = "{\"Property1\":\"Value1\",\"Property2\":42}";
        var additionalDescription = "Sample Description";
        var mediatorSerializedObject = new MediatorSerializedObject(fullTypeName, data, additionalDescription);

        // Act
        var success = mediatorSerializedObject.TryDeserializeObject(out var deserializedObject);

        // Assert
        success.Should().BeTrue();
        deserializedObject.Should().NotBeNull();
        deserializedObject.Should().BeOfType<SampleRequest>();
        var sampleRequest = (SampleRequest)deserializedObject!;
        sampleRequest.Property1.Should().Be("Value1");
        sampleRequest.Property2.Should().Be(42);
    }

    [Fact]
    public void TryDeserializeObject_WithInvalidData_ShouldReturnFalse()
    {
        // Arrange
        var fullTypeName = SampleRequestTypeName;
        var data = "{\"Property1\":\"Value1\",\"InvalidProperty\":42}";
        var additionalDescription = "Sample Description";
        var mediatorSerializedObject = new MediatorSerializedObject(fullTypeName, data, additionalDescription);

        // Act
        bool success = mediatorSerializedObject.TryDeserializeObject(out var deserializedObject);

        // Assert
        success.Should().BeFalse();
        deserializedObject.Should().BeNull();
    }

    [Fact]
    public void SerializeObject_ShouldReturnMediatorSerializedObject()
    {
        // Arrange
        var mediatorObject = new SampleRequest
        {
            Property1 = "Value1",
            Property2 = 42
        };
        const string description = "Sample Description";

        // Act
        var result = MediatorSerializedObject.SerializeObject(mediatorObject, description);

        // Assert
        result.Should().NotBeNull();
        result.AssemblyQualifiedName.Should().Be(mediatorObject.GetType().AssemblyQualifiedName);
        result.Data.Should().Be("{\"Property1\":\"Value1\",\"Property2\":42}");
        result.AdditionalDescription.Should().Be("Sample Description");
    }

    private class SampleRequest : IBaseRequest
    {
        public string Property1 { get; set; } = null!;
        public int Property2 { get; set; }
    }
}
