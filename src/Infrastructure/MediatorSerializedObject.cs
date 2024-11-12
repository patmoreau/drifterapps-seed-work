using System.Text.Json;
using MediatR;

namespace DrifterApps.Seeds.Infrastructure;

internal class MediatorSerializedObject
{
    public MediatorSerializedObject(string assemblyQualifiedName, string data, string additionalDescription)
    {
        if (string.IsNullOrWhiteSpace(assemblyQualifiedName))
        {
            throw new ArgumentNullException(nameof(assemblyQualifiedName),
                $"Missing assembly name for '{additionalDescription}'");
        }

        _ = Type.GetType(assemblyQualifiedName, false) ??
            throw new ArgumentException("Invalid Type name", assemblyQualifiedName);
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

    internal bool TryDeserializeObject(out IBaseRequest? request, JsonSerializerOptions jsonSerializerOptions)
    {
        request = default;

        var type = Type.GetType(AssemblyQualifiedName);

        try
        {
            var req = JsonSerializer.Deserialize(Data, type!, jsonSerializerOptions);
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
        string description, JsonSerializerOptions jsonSerializerOptions) where TBaseRequest : IBaseRequest
    {
        var type = mediatorObject.GetType();
        var assemblyName = type.AssemblyQualifiedName!;
        var data = JsonSerializer.Serialize(mediatorObject, jsonSerializerOptions);

        return new MediatorSerializedObject(assemblyName, data, description);
    }

    private bool Equals(MediatorSerializedObject other) =>
        AssemblyQualifiedName == other.AssemblyQualifiedName &&
        Data == other.Data &&
        AdditionalDescription == other.AdditionalDescription;

    public override bool Equals(object? obj) =>
        obj is not null && (ReferenceEquals(this, obj) ||
                            (obj.GetType() == GetType() &&
                             Equals((MediatorSerializedObject) obj)));

    public override int GetHashCode() => HashCode.Combine(AssemblyQualifiedName, Data, AdditionalDescription);
}
