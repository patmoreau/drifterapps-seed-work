using Microsoft.EntityFrameworkCore;

namespace DrifterApps.Seeds.Testing.Drivers;

public abstract partial class DatabaseDriver<TDbContext> where TDbContext : DbContext
{
    public string ConnectionString => DatabaseServer.ConnectionString;

    public abstract TDbContext CreateDbContext();

    public abstract DbContextOptions<TDbContext> GetDbContextOptions();

    protected abstract Task InitializeDatabaseAsync();
}
