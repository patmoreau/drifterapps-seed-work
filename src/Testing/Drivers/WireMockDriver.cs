using WireMock.Server;

namespace DrifterApps.Seeds.Testing.Drivers;

/// <summary>
///     Abstract base class for managing a WireMock server lifecycle.
///     Implements the IAsyncLifetime interface for asynchronous initialization and disposal.
/// </summary>
public abstract class WireMockDriver : IAsyncLifetime
{
    private WireMockServer? _server;

    /// <summary>
    ///     Gets the initialized WireMock server instance.
    ///     Throws an InvalidOperationException if the server is not initialized.
    /// </summary>
    protected WireMockServer Server =>
        _server ?? throw new InvalidOperationException("WireMockServer is not initialized.");

    /// <summary>
    ///     Initializes the WireMock server asynchronously.
    /// </summary>
    /// <returns>A completed task.</returns>
    public virtual Task InitializeAsync()
    {
        _server = WireMockServer.Start();

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Disposes of the WireMock server asynchronously.
    /// </summary>
    /// <returns>A completed task.</returns>
    public virtual Task DisposeAsync()
    {
        _server?.Stop();
        _server?.Dispose();
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Configures the WireMock server.
    ///     Must be implemented by derived classes.
    /// </summary>
    protected abstract void Configure();
}
