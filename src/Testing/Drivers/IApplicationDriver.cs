// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Xunit.Abstractions;

namespace DrifterApps.Seeds.Tests.Drivers;

public interface IApplicationDriver
{
    HttpClientDriver CreateHttpClientDriver(ITestOutputHelper testOutputHelper);

    Task OnScenarioReset();
}
