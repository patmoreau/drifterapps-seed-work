// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Bogus;
using DrifterApps.Seeds.Testing.Drivers;
using Microsoft.EntityFrameworkCore;

namespace DrifterApps.Seeds.Testing;

public abstract class FakerBuilder<T> where T : class
{
    /// <inheritdoc cref="Faker{T}" />
    protected abstract Faker<T> FakerRules { get; }

    public virtual T Build()
    {
        FakerRules.AssertConfigurationIsValid();
        return FakerRules.Generate();
    }

    public virtual IReadOnlyCollection<T> BuildCollection(int? count = null)
    {
        FakerRules.AssertConfigurationIsValid();
        return FakerRules.Generate(count ?? Globals.RandomCollectionCount());
    }

    public Task<T> SavedInDbAsync<TDbContext>(DatabaseDriver<TDbContext> databaseDriver) where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(databaseDriver);

        return SavedInDbInternalAsync(databaseDriver);
    }

    private async Task<T> SavedInDbInternalAsync<TDbContext>(DatabaseDriver<TDbContext> databaseDriver)
        where TDbContext : DbContext
    {
        var dbContext = databaseDriver.CreateDbContext();
        await using (dbContext.ConfigureAwait(false))
        {
            var entity = Build();
            await dbContext.SaveAsync(entity).ConfigureAwait(false);
            return entity;
        }
    }

    public Task<IReadOnlyCollection<T>> CollectionSavedInDbAsync<TDbContext>(DatabaseDriver<TDbContext> databaseDriver,
        int? count = null) where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(databaseDriver);

        return CollectionSavedInDbInternalAsync(databaseDriver, count);
    }

    private async Task<IReadOnlyCollection<T>> CollectionSavedInDbInternalAsync<TDbContext>(
        DatabaseDriver<TDbContext> databaseDriver,
        int? count = null) where TDbContext : DbContext
    {
        var dbContext = databaseDriver.CreateDbContext();
        await using (dbContext.ConfigureAwait(false))
        {
            var entities = BuildCollection(count);
            foreach (var entity in entities) await dbContext.SaveAsync(entity).ConfigureAwait(false);

            return entities;
        }
    }
}
