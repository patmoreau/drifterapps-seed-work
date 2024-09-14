using DrifterApps.Seeds.Testing.Drivers;
using Microsoft.EntityFrameworkCore;

namespace DrifterApps.Seeds.Testing;

public abstract partial class FakerBuilder<TFaked>
{
    /// <summary>
    ///     Asynchronously saves an instance of <typeparamref name="TFaked" /> in the database.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    /// <param name="databaseDriver">The database driver to use for saving the instance.</param>
    /// <returns>
    ///     A task that represents the asynchronous save operation. The task result contains the saved instance of
    ///     <typeparamref name="TFaked" />.
    /// </returns>
    public Task<TFaked> SavedInDbAsync<TDbContext>(DatabaseDriver<TDbContext> databaseDriver)
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(databaseDriver);

        return SavedInDbInternalAsync(databaseDriver);
    }

    private async Task<TFaked> SavedInDbInternalAsync<TDbContext>(DatabaseDriver<TDbContext> databaseDriver)
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
    ///     Asynchronously saves a collection of instances of <typeparamref name="TFaked" /> in the database.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    /// <param name="databaseDriver">The database driver to use for saving the instances.</param>
    /// <param name="count">Optional number of entities to generate. If not provided, a random count will be used.</param>
    /// <returns>
    ///     A task that represents the asynchronous save operation. The task result contains a read-only collection of
    ///     saved instances of <typeparamref name="TFaked" />.
    /// </returns>
    public Task<IReadOnlyCollection<TFaked>> CollectionSavedInDbAsync<TDbContext>(
        DatabaseDriver<TDbContext> databaseDriver,
        int? count = null) where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(databaseDriver);

        return CollectionSavedInDbInternalAsync(databaseDriver, count);
    }

    private async Task<IReadOnlyCollection<TFaked>> CollectionSavedInDbInternalAsync<TDbContext>(
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
