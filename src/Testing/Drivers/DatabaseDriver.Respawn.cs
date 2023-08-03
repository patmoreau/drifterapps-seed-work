// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Respawn;

namespace DrifterApps.Seeds.Testing.Drivers;

public abstract partial class DatabaseDriver<TDbContext> : IRespawnable
{
    private Respawner? _respawner;

    protected abstract RespawnerOptions Options { get; }

    public async Task ResetCheckpointAsync()
    {
        if (_respawner is null) throw new InvalidOperationException("The database has not been initialized.");

        var connection = await DatabaseServer.GetConnectionAsync().ConfigureAwait(false);
        await using (connection)
        {
            await _respawner.ResetAsync(connection).ConfigureAwait(false);
        }
    }

    private async Task InitialiseRespawnAsync()
    {
        var connection = await DatabaseServer.GetConnectionAsync().ConfigureAwait(false);
        await using (connection)
        {
            _respawner = await Respawner.CreateAsync(connection, Options).ConfigureAwait(false);
        }
    }
}
