using System.Data.Common;

namespace DrifterApps.Seeds.Testing.Infrastructure.Persistence;

public class LocalDatabaseServer : IDatabaseServer
{
    public delegate DbConnection ConnectionFactory(string connectionString);

    private readonly ConnectionFactory _connectionFactory;

    private LocalDatabaseServer(string connectionString, ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        ConnectionString = connectionString;
    }

    /// <inheritdoc />
    public string ConnectionString { get; }

    /// <inheritdoc />
    public Task StartAsync() => Task.CompletedTask;

    /// <inheritdoc />
    public async Task<DbConnection> GetConnectionAsync()
    {
        var connection = _connectionFactory(ConnectionString);
        await connection.OpenAsync().ConfigureAwait(false);

        return connection;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public static LocalDatabaseServer CreateServer(string connectionString, ConnectionFactory connectionFactory) =>
        new(connectionString, connectionFactory);
}
