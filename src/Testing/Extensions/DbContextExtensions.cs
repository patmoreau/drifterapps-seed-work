// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class DbContextExtensions
{
    public static async Task SaveAsync<T>(this DbContext dbContext, T entity) where T : class
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        await dbContext.AddAsync(entity).ConfigureAwait(false);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public static async Task<T?> FindByIdAsync<T>(this DbContext dbContext, Guid id) where T : class
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        RefreshAll(dbContext);
        return await dbContext.FindAsync<T>(id).ConfigureAwait(false);
    }

    private static void RefreshAll(DbContext dbContext)
    {
        var entitiesList = dbContext.ChangeTracker.Entries().ToList();
        foreach (var entity in entitiesList)
        {
            entity.Reload();
        }
    }
}
