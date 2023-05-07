// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace DrifterApps.Seeds.Tests;

public interface IDriverOfT<out TSystemUnderTest> where TSystemUnderTest : class
{
    TSystemUnderTest Build();
}
