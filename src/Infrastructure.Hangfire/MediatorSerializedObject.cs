namespace DrifterApps.Seeds.Application.Hangfire;

public class MediatorSerializedObject
{
    public MediatorSerializedObject(string fullTypeName, string data, string additionalDescription)
    {
        FullTypeName = fullTypeName;
        Data = data;
        AdditionalDescription = additionalDescription;
    }

    public string FullTypeName { get; }

    public string Data { get; }

    public string AdditionalDescription { get; }

    /// <summary>
    ///     Override for Hangfire dashboard display.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var commandName = FullTypeName.Split('.').Last();
        return $"{commandName} {AdditionalDescription}";
    }
}
