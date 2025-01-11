using System.Data.Common;
using DotNet.Testcontainers.Builders;
using Npgsql;
using Testcontainers.PostgreSql;

namespace DrifterApps.Seeds.Testing.Infrastructure.Persistence;

/// <summary>
///     Represents a PostgreSQL database server for testing purposes.
/// </summary>
public class PostgreDatabaseServer : IDatabaseServer
{
    private const string RootUser = "root";
    private const string RootPassword = "root_password";
    private readonly PostgreSqlContainer _container;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PostgreDatabaseServer" /> class.
    /// </summary>
    /// <param name="databaseName">The name of the database.</param>
    /// <param name="port">The port to bind the database server to (optional).</param>
    /// <param name="image">The Docker image to use for the database server (optional).</param>
    private PostgreDatabaseServer(string databaseName, int? port = null, string? image = null)
    {
        var builder = new PostgreSqlBuilder()
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

    /// <summary>
    ///     Gets the connection string for the PostgreSQL database server.
    /// </summary>
    public string ConnectionString => _container.GetConnectionString();

    /// <summary>
    ///     Asynchronously gets a database connection.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the database connection.</returns>
    public async Task<DbConnection> GetConnectionAsync()
    {
        var connection = new NpgsqlConnection(_container.GetConnectionString());
        await connection.OpenAsync().ConfigureAwait(false);

        return connection;
    }

    /// <summary>
    ///     Asynchronously starts the PostgreSQL database server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task StartAsync() => await _container.StartAsync().ConfigureAwait(false);

    /// <summary>
    ///     Asynchronously disposes the PostgreSQL database server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="PostgreDatabaseServer" /> class.
    /// </summary>
    /// <param name="databaseName">The name of the database.</param>
    /// <returns>A new instance of the <see cref="PostgreDatabaseServer" /> class.</returns>
    public static PostgreDatabaseServer CreateServer(string databaseName)
        => new(databaseName);

    /// <summary>
    ///     Creates a new instance of the <see cref="PostgreDatabaseServer" /> class with a specified Docker image.
    /// </summary>
    /// <param name="databaseName">The name of the database.</param>
    /// <param name="image">The Docker image to use for the database server.</param>
    /// <returns>A new instance of the <see cref="PostgreDatabaseServer" /> class.</returns>
    public static PostgreDatabaseServer CreateServer(string databaseName, string image)
        => new(databaseName, image: image);

    /// <summary>
    ///     Creates a new instance of the <see cref="PostgreDatabaseServer" /> class with a specified port.
    /// </summary>
    /// <param name="databaseName">The name of the database.</param>
    /// <param name="port">The port to bind the database server to.</param>
    /// <returns>A new instance of the <see cref="PostgreDatabaseServer" /> class.</returns>
    public static PostgreDatabaseServer CreateServer(string databaseName, int port)
        => new(databaseName, port);

    /// <summary>
    ///     Creates a new instance of the <see cref="PostgreDatabaseServer" /> class with a specified port and Docker image.
    /// </summary>
    /// <param name="databaseName">The name of the database.</param>
    /// <param name="port">The port to bind the database server to.</param>
    /// <param name="image">The Docker image to use for the database server.</param>
    /// <returns>A new instance of the <see cref="PostgreDatabaseServer" /> class.</returns>
    public static PostgreDatabaseServer CreateServer(string databaseName, int port, string image)
        => new(databaseName, port, image);
}
