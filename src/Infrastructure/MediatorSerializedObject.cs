using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;

namespace DrifterApps.Seeds.Infrastructure;

internal class MediatorSerializedObject
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    public MediatorSerializedObject(string assemblyQualifiedName, string data, string additionalDescription)
    {
        _ = Type.GetType(assemblyQualifiedName) ??
            throw new ArgumentException("Invalid Type name", assemblyQualifiedName);
        AssemblyQualifiedName = assemblyQualifiedName;
        Data = data;
        AdditionalDescription = additionalDescription;
    }

    private string AssemblyQualifiedName { get; }

    private string Data { get; }

    private string AdditionalDescription { get; }

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
            var req = JsonSerializer.Deserialize(Data, type!, Options);
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
        var data = JsonSerializer.Serialize(mediatorObject, Options);

        return new MediatorSerializedObject(assemblyName, data, description);
    }
}
