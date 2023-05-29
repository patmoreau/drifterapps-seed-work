using System.Text.Json;
using MediatR;
using Xunit.Categories;

namespace DrifterApps.Seeds.Infrastructure;

[UnitTest]
internal class MediatorSerializedObject
{
    public MediatorSerializedObject(string assemblyQualifiedName, string data, string additionalDescription)
    {
        var type = Type.GetType(assemblyQualifiedName);
        if (type is null)
        {
            throw new ArgumentException("Invalid Type name", assemblyQualifiedName);
        }
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

        if (type == null) return false;

        var req = JsonSerializer.Deserialize(Data, type);
        switch (req)
        {
            case IBaseRequest baseRequest:
                request = baseRequest;
                return true;
            default:
                return false;
        }
    }

    internal static MediatorSerializedObject SerializeObject(object mediatorObject, string description)
    {
        var type = mediatorObject.GetType();
        var assemblyName = type.AssemblyQualifiedName!;
        var data = JsonSerializer.Serialize(mediatorObject);

        return new MediatorSerializedObject(assemblyName, data, description);
    }
}
