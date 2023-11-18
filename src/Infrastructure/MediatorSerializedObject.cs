using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;

namespace DrifterApps.Seeds.Infrastructure;

internal class MediatorSerializedObject
{
    private static readonly JsonSerializerOptions _options = new()
    {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            };

    public MediatorSerializedObject(string assemblyQualifiedName, string data, string additionalDescription)
    {
        var type = Type.GetType(assemblyQualifiedName);
        if (type is null) throw new ArgumentException("Invalid Type name", assemblyQualifiedName);

        AssemblyQualifiedName = assemblyQualifiedName;
        Data = data;
        AdditionalDescription = additionalDescription;
    }

    public string AssemblyQualifiedName { get; }

    public string Data { get; }

    public string AdditionalDescription { get; }

    /// <summary>
    ///     Override for Hangfire dashboard display.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var type = Type.GetType(AssemblyQualifiedName);
        var requestName = type!.Name;
        return $"{requestName} {AdditionalDescription}";
    }

    internal bool TryDeserializeObject(out IBaseRequest? request)
    {
        request = default;

        var type = Type.GetType(AssemblyQualifiedName);

        try
        {
            var req = JsonSerializer.Deserialize(Data, type!, _options);
            switch (req)
            {
                case IBaseRequest baseRequest:
                    request = baseRequest;
                    return true;
                default:
                    return false;
            }
        }
        catch (JsonException)
        {
            return false;
        }
    }

    internal static MediatorSerializedObject SerializeObject<TBaseRequest>(TBaseRequest mediatorObject,
        string description) where TBaseRequest : IBaseRequest
    {
        var type = mediatorObject.GetType();
        var assemblyName = type.AssemblyQualifiedName!;
        var data = JsonSerializer.Serialize(mediatorObject, _options);

        return new MediatorSerializedObject(assemblyName, data, description);
    }
}
