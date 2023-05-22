// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Data.Common;

namespace DrifterApps.Seeds.Testing.Infrastructure.Persistence;

public interface IDatabaseServer : IAsyncDisposable
{
    string ConnectionString { get; }

    Task StartAsync();

    Task GetConnectionAsync(Func<DbConnection, Task> action);
}
