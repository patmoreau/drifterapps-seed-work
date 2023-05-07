using System.Data.Common;

namespace DrifterApps.Seeds.Tests.Infrastructure.Persistence;

public class LocalDatabaseServer : IDatabaseServer
{
    private readonly Func<string, DbConnection> _connectionFactory;

    public LocalDatabaseServer(string connectionString, Func<string, DbConnection> connectionFactory)
    {
        _connectionFactory = connectionFactory;
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }

    public Task StartAsync() => Task.CompletedTask;

    public Task GetConnectionAsync(Func<DbConnection, Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        return GetConnectionInternalAsync(action);
    }

    private async Task GetConnectionInternalAsync(Func<DbConnection, Task> action)
    {
        using var connection = _connectionFactory(ConnectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        await action(connection).ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
