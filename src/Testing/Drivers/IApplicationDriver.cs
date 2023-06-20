// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Xunit.Abstractions;

namespace DrifterApps.Seeds.Testing.Drivers;

public interface IApplicationDriver
{
    IServiceProvider Services { get; }

    IHttpClientDriver CreateHttpClientDriver(ITestOutputHelper testOutputHelper);

    Task ResetStateAsync();
}
