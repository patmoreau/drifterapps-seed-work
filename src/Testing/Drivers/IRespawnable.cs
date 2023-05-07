// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace DrifterApps.Seeds.Tests.Drivers;

public interface IRespawnable
{
    Task ResetCheckpointAsync();
}
