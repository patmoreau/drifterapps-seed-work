// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace DrifterApps.Seeds.Testing;

public interface IDriverOf<out TSystemUnderTest> where TSystemUnderTest : class
{
    TSystemUnderTest Build();
}
