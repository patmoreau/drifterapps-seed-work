// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Bogus;
using DrifterApps.Seeds.Testing.Drivers;

namespace DrifterApps.Seeds.Testing;

public abstract class FakerBuilder<T> where T : class
{
    /// <inheritdoc cref="Faker{T}" />
    protected abstract Faker<T> Faker { get; }

    public virtual T Build()
    {
        Faker.AssertConfigurationIsValid();
        return Faker.Generate();
    }

    public virtual IReadOnlyCollection<T> BuildCollection(int? count = null)
    {
        Faker.AssertConfigurationIsValid();
        return Faker.Generate(count ?? Globals.RandomCollectionCount());
    }

    public Task<T> SavedInDbAsync(ISaveBuilder databaseDriver)
    {
        ArgumentNullException.ThrowIfNull(databaseDriver);

        return SavedInDbInternalAsync(databaseDriver);
    }

    private async Task<T> SavedInDbInternalAsync(ISaveBuilder databaseDriver)
    {
        var entity = Build();
        await databaseDriver.SaveAsync(entity).ConfigureAwait(false);
        return entity;
    }

    public Task<IReadOnlyCollection<T>> CollectionSavedInDbAsync(ISaveBuilder databaseDriver, int? count = null)
    {
        ArgumentNullException.ThrowIfNull(databaseDriver);

        return CollectionSavedInDbInternalAsync(databaseDriver, count);
    }

    private async Task<IReadOnlyCollection<T>> CollectionSavedInDbInternalAsync(ISaveBuilder databaseDriver,
        int? count = null)
    {
        var entities = BuildCollection(count);
        foreach (var entity in entities) await databaseDriver.SaveAsync(entity).ConfigureAwait(false);

        return entities;
    }
}
