// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;

namespace DrifterApps.Seeds.Testing.Drivers;

public abstract partial class DatabaseDriver<TDbContext> : ISaveBuilder where TDbContext : DbContext
{
    public abstract TDbContext DbContext { get; protected set; }

    public string ConnectionString => DatabaseServer.ConnectionString;

    public async Task SaveAsync<T>(T entity) where T : class
    {
        DbContext.Add(entity);
        await DbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<T?> FindByIdAsync<T>(Guid id) where T : class
    {
        RefreshAll();
        return await DbContext.FindAsync<T>(id).ConfigureAwait(false);
    }

    public abstract DbContextOptions<TDbContext> GetDbContextOptions();

    protected abstract Task InitializeDatabaseAsync();

    private void RefreshAll()
    {
        var entitiesList = DbContext.ChangeTracker.Entries().ToList();
        foreach (var entity in entitiesList) entity.Reload();
    }
}
