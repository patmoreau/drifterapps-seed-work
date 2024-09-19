using System.Text.Json;
using System.Text.Json.Serialization;
using DrifterApps.Seeds.Domain;

namespace DrifterApps.Seeds.Application.Converters;

/// <summary>
///     JSON converter for strongly-typed IDs.
/// </summary>
/// <typeparam name="T">The type of the strongly-typed ID.</typeparam>
public class StronglyTypedIdJsonConverter<T> : JsonConverter<StronglyTypedId<T>> where T : StronglyTypedId<T>, new()
{
    /// <summary>
    ///     Determines whether the specified type can be converted.
    /// </summary>
    /// <param name="typeToConvert">The type to check.</param>
    /// <returns>True if the type can be converted; otherwise, false.</returns>
    public override bool CanConvert(Type typeToConvert) => typeof(IStronglyTypedId).IsAssignableFrom(typeToConvert);

    /// <summary>
    ///     Reads and converts the JSON to a strongly-typed ID.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The strongly-typed ID.</returns>
    public override StronglyTypedId<T> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetGuid();
        return StronglyTypedId<T>.Create(value);
    }

    /// <summary>
    ///     Writes the strongly-typed ID as a JSON string.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The strongly-typed ID value.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, StronglyTypedId<T> value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);
        writer.WriteStringValue(value.Value);
    }
}
