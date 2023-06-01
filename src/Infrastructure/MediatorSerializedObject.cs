using MediatR;
using Newtonsoft.Json;

namespace DrifterApps.Seeds.Infrastructure;

internal class MediatorSerializedObject
{
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
            var req = JsonConvert.DeserializeObject(Data, type, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error
            });
            switch (req)
            {
                case IBaseRequest baseRequest:
                    request = baseRequest;
                    return true;
                default:
                    return false;
            }
        }
        catch (JsonSerializationException)
        {
            return false;
        }
    }

    internal static MediatorSerializedObject SerializeObject(IBaseRequest mediatorObject, string description)
    {
        var type = mediatorObject.GetType();
        var assemblyName = type.AssemblyQualifiedName!;
        var data = JsonConvert.SerializeObject(mediatorObject);

        return new MediatorSerializedObject(assemblyName, data, description);
    }
}
