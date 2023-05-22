// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Data.Common;
using MySqlConnector;
using Testcontainers.MariaDb;

namespace DrifterApps.Seeds.Testing.Infrastructure.Persistence;

public class MariaDatabaseServer : IDatabaseServer
{
    private readonly MariaDbContainer _container;

    private MariaDatabaseServer(string databaseName)
    {
        _container = new MariaDbBuilder()
            .WithDatabase(databaseName)
            .WithUsername("root")
            .WithPassword("root")
            .Build();
    }

    public string ConnectionString => _container.GetConnectionString();

    public Task StartAsync() => _container.StartAsync();

    public Task GetConnectionAsync(Func<DbConnection, Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        return GetConnectionInternalAsync(action);
    }

    private async Task GetConnectionInternalAsync(Func<DbConnection, Task> action)
    {
        using var connection = new MySqlConnection(_container.GetConnectionString());
        await connection.OpenAsync().ConfigureAwait(false);
        await action(connection).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public static MariaDatabaseServer CreateServer(string databaseName) => new(databaseName);
}
