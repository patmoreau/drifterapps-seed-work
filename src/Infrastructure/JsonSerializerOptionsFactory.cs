using System.Text.Json;
using System.Text.Json.Serialization;

namespace DrifterApps.Seeds.Infrastructure;

internal interface IJsonSerializerOptionsFactory
{
    JsonSerializerOptions CreateOptions();
}

internal class JsonSerializerOptionsFactory(Func<JsonSerializerOptions>? factory)
    : IJsonSerializerOptionsFactory
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    public JsonSerializerOptions CreateOptions() => factory?.Invoke() ?? DefaultOptions;
}
