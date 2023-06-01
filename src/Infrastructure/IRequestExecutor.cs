namespace DrifterApps.Seeds.Infrastructure;

internal interface IRequestExecutor
{
    Task ExecuteCommandAsync(MediatorSerializedObject mediatorSerializedObject);
}
