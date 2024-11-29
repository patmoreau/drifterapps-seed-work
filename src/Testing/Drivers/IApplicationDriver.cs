namespace DrifterApps.Seeds.Testing.Drivers;

public interface IApplicationDriver
{
    IServiceProvider Services { get; }

    Task ResetStateAsync();
}
