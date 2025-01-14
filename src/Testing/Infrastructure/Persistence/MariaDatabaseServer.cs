using System.Data.Common;
using DotNet.Testcontainers.Builders;
using MySqlConnector;
using Testcontainers.MariaDb;

namespace DrifterApps.Seeds.Testing.Infrastructure.Persistence;

public class MariaDatabaseServer : IDatabaseServer
{
    private const string RootUser = "root";
    private const string RootPassword = "root_password";
    private readonly MariaDbContainer _container;

    private MariaDatabaseServer(string databaseName, int? port = null, string? image = null)
    {
        var builder = new MariaDbBuilder()
            .WithDatabase(databaseName)
            .WithUsername(RootUser)
            .WithPassword(RootPassword)
            .WithWaitStrategy(Wait.ForUnixContainer());

        if (port is not null)
        {
            builder = builder.WithPortBinding(port.Value);
        }

        if (!string.IsNullOrWhiteSpace(image))
        {
            builder = builder.WithImage(image);
        }

        _container = builder.Build();
    }

    public string ConnectionString => _container.GetConnectionString();

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

    public static MariaDatabaseServer CreateServer(string databaseName, string image) =>
        new(databaseName, image: image);

    public static MariaDatabaseServer CreateServer(string databaseName, int port) => new(databaseName, port);

    public static MariaDatabaseServer CreateServer(string databaseName, int port, string image) =>
        new(databaseName, port, image);
}
