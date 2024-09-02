using System.Text.Json;
using System.Text.Json.Serialization;

namespace DrifterApps.Seeds.Domain;

public class StronglyTypedIdJsonConverter<T> : JsonConverter<StronglyTypedId<T>> where T : StronglyTypedId<T>, new()
{
    public override bool CanConvert(Type typeToConvert) => typeof(IStronglyTypedId).IsAssignableFrom(typeToConvert);

    public override StronglyTypedId<T> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetGuid();
        return StronglyTypedId<T>.Create(value);
    }

    public override void Write(Utf8JsonWriter writer, StronglyTypedId<T> value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);
        writer.WriteStringValue(value.Value);
    }
}
