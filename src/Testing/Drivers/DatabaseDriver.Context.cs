// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;

namespace DrifterApps.Seeds.Testing.Drivers;

public abstract partial class DatabaseDriver<TDbContext> where TDbContext : DbContext
{
    public string ConnectionString => DatabaseServer.ConnectionString;

    public abstract TDbContext CreateDbContext();

    public abstract DbContextOptions<TDbContext> GetDbContextOptions();

    protected abstract Task InitializeDatabaseAsync();
}
