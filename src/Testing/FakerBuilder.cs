// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Bogus;
using DrifterApps.Seeds.Testing.Drivers;
using Microsoft.EntityFrameworkCore;

namespace DrifterApps.Seeds.Testing;

/// <summary>
///     Abstract base class for building instances of <typeparamref name="T" /> using Faker.
/// </summary>
/// <typeparam name="T">The type of class to generate instances for.</typeparam>
public abstract class FakerBuilder<T> where T : class
{
    /// <summary>
    ///     Configures the rules for generating instances of <typeparamref name="T" />.
    /// </summary>
    /// <param name="fakerBuilder">The Faker builder to configure.</param>
    /// <returns>The configured Faker builder.</returns>
    protected abstract Faker<T> ConfigureRules(Faker<T> fakerBuilder);

    /// <summary>
    ///     Builds an instance of <typeparamref name="T" /> using the configured rules.
    /// </summary>
    /// <returns>An instance of <typeparamref name="T" />.</returns>
    public T Build()
    {
        var builder = ConfigureRules(new Faker<T>());
        builder.AssertConfigurationIsValid();
        return builder.Generate();
    }

    /// <summary>
    ///     Build a collection of entities.
    /// </summary>
    /// <param name="count">
    ///     Optional number of entities to generate. If not provided, a random count will be used.
    /// </param>
    /// <returns>
    ///     A read-only collection of entities of type <typeparamref name="T" />.
    /// </returns>
    public IReadOnlyCollection<T> BuildCollection(int? count = null)
    {
        var builder = ConfigureRules(new Faker<T>());
        builder.AssertConfigurationIsValid();
        return builder.Generate(count ?? Globals.RandomCollectionCount());
    }

    /// <summary>
    ///     Asynchronously saves an instance of <typeparamref name="T" /> in the database.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    /// <param name="databaseDriver">The database driver to use for saving the instance.</param>
    /// <returns>
    ///     A task that represents the asynchronous save operation. The task result contains the saved instance of
    ///     <typeparamref name="T" />.
    /// </returns>
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

    /// <summary>
    ///     Asynchronously saves a collection of instances of <typeparamref name="T" /> in the database.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    /// <param name="databaseDriver">The database driver to use for saving the instances.</param>
    /// <param name="count">Optional number of entities to generate. If not provided, a random count will be used.</param>
    /// <returns>
    ///     A task that represents the asynchronous save operation. The task result contains a read-only collection of
    ///     saved instances of <typeparamref name="T" />.
    /// </returns>
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
            foreach (var entity in entities)
            {
                await dbContext.SaveAsync(entity).ConfigureAwait(false);
            }

            return entities;
        }
    }
}
