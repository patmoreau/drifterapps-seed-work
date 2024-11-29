using System.Data.Common;

namespace DrifterApps.Seeds.Testing.Infrastructure.Persistence;

public interface IDatabaseServer : IAsyncDisposable
{
    string ConnectionString { get; }

    Task StartAsync();

    Task<DbConnection> GetConnectionAsync();
}
