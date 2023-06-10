// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Data.Common;
using DotNet.Testcontainers.Builders;
using MySqlConnector;
using Testcontainers.MariaDb;

namespace DrifterApps.Seeds.Testing.Infrastructure.Persistence;

public class MariaDatabaseServer : IDatabaseServer
{
    private readonly MariaDbContainer _container;

    public string ConnectionString => _container.GetConnectionString();

    private MariaDatabaseServer(string databaseName) =>
        _container = new MariaDbBuilder()
            .WithDatabase(databaseName)
            .WithUsername("root")
            .WithPassword("root")
            .WithWaitStrategy(Wait.ForUnixContainer())
            .Build();

    public async Task<DbConnection> GetConnectionAsync()
    {
        var connection = new MySqlConnection(_container.GetConnectionString());
        await connection.OpenAsync().ConfigureAwait(false);

        return connection;
    }

    public async Task StartAsync() => await _container.StartAsync().ConfigureAwait(false);

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public static MariaDatabaseServer CreateServer(string databaseName) => new(databaseName);
}
