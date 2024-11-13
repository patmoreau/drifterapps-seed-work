using DrifterApps.Seeds.Domain;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore;
#pragma warning restore IDE0130

public static class DbContextExtensions
{
    public static async Task SaveAsync<T>(this DbContext dbContext, T entity) where T : class
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        await dbContext.AddAsync(entity).ConfigureAwait(false);
        await dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public static async Task<T?> FindByIdAsync<T>(this DbContext dbContext, object id) where T : class
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        RefreshAll(dbContext);
        return id is IStronglyTypedId stronglyTypedId
            ? await dbContext.FindAsync<T>(stronglyTypedId.Value).ConfigureAwait(false)
            : await dbContext.FindAsync<T>(id).ConfigureAwait(false);
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
