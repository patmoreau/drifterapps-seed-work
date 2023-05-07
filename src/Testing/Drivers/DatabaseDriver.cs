using DrifterApps.Seeds.Tests.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DrifterApps.Seeds.Tests.Drivers;

public abstract partial class DatabaseDriver<TDbContext> : IAsyncLifetime where TDbContext : DbContext
{
    protected abstract IDatabaseServer DatabaseServer { get; init; }

    public virtual async Task InitializeAsync()
    {
        await InitializeDatabaseAsync().ConfigureAwait(false);
        await InitialiseRespawnAsync().ConfigureAwait(false);
    }

    public virtual async Task DisposeAsync()
    {
        await DatabaseServer.DisposeAsync().ConfigureAwait(false);
    }
}
