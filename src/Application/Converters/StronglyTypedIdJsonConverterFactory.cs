using System.Text.Json;
using System.Text.Json.Serialization;
using DrifterApps.Seeds.Domain;

namespace DrifterApps.Seeds.Application.Converters;

/// <summary>
///     A factory for creating JSON converters for strongly-typed IDs.
/// </summary>
public class StronglyTypedIdJsonConverterFactory : JsonConverterFactory
{
    /// <summary>
    ///     Determines whether the specified type can be converted.
    /// </summary>
    /// <param name="typeToConvert">The type to check.</param>
    /// <returns>True if the type can be converted; otherwise, false.</returns>
    public override bool CanConvert(Type typeToConvert) => typeof(IStronglyTypedId).IsAssignableFrom(typeToConvert);

    /// <summary>
    ///     Creates a JSON converter for the specified type.
    /// </summary>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>A JSON converter for the specified type.</returns>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter) Activator.CreateInstance(
            typeof(StronglyTypedIdJsonConverter<>).MakeGenericType(typeToConvert))!;
}
