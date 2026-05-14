using DrifterApps.Seeds.Testing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrifterApps.Seeds.Testing.Drivers;

public abstract partial class DatabaseDriver<TDbContext> : IAsyncLifetime where TDbContext : DbContext
{
    protected abstract IDatabaseServer DatabaseServer { get; init; }

    public virtual async ValueTask InitializeAsync()
    {
        await InitializeDatabaseAsync().ConfigureAwait(false);
        await InitialiseRespawnAsync().ConfigureAwait(false);
    }

    public virtual async ValueTask DisposeAsync()
    {
        await DatabaseServer.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
