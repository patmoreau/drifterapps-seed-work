using System.Text.Json;
using DrifterApps.Seeds.Application.Converters;
using DrifterApps.Seeds.Domain;

namespace DrifterApps.Seeds.Application.Tests.Converters;

[UnitTest]
public class StronglyTypedIdJsonConverterOfTTests
{
    private readonly JsonSerializerOptions _options = new()
    {
        Converters = {new StronglyTypedIdJsonConverter<SampleStronglyTypedId>()}
    };

    [Fact]
    public void GivenCanConvert_WhenAssignableType_ThenReturnsTrue()
    {
        // assign
        var converter = new StronglyTypedIdJsonConverter<SampleStronglyTypedId>();

        // act
        var result = converter.CanConvert(typeof(SampleStronglyTypedId));

        // assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GivenCanConvert_WhenNonAssignableType_ThenReturnsFalse()
    {
        // assign
        var converter = new StronglyTypedIdJsonConverter<SampleStronglyTypedId>();

        // act
        var result = converter.CanConvert(typeof(string));

        // assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GivenRead_WhenValidGuid_ThenReturnsStronglyTypedId()
    {
        // arrange
        const string json = "\"d2719b2e-4b8b-4c8b-9b2e-4b8b4c8b9b2e\"";

        // act
        var result = JsonSerializer.Deserialize<SampleStronglyTypedId>(json, _options);

        // assert
        result.Should().NotBeNull().And.BeOfType<SampleStronglyTypedId>();
        result!.Value.Should().Be(Guid.Parse("d2719b2e-4b8b-4c8b-9b2e-4b8b4c8b9b2e"));
    }

    [Fact]
    public void GivenRead_WhenInvalidGuid_ThenThrowJsonException()
    {
        // arrange
        const string json = "\"invalid-guid\"";

        // act
        Action act = () => _ = JsonSerializer.Deserialize<SampleStronglyTypedId>(json, _options);

        // assert
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void GivenWrite_WhenValidGuid_ThenAsString()
    {
        // arrange
        var stronglyTypedId = SampleStronglyTypedId.Create(Guid.Parse("d2719b2e-4b8b-4c8b-9b2e-4b8b4c8b9b2e"));

        // act
        var json = JsonSerializer.Serialize(stronglyTypedId, _options);

        // assert
        json.Should().Be("\"d2719b2e-4b8b-4c8b-9b2e-4b8b4c8b9b2e\"");
    }

    internal record SampleStronglyTypedId : StronglyTypedId<SampleStronglyTypedId>;
}
