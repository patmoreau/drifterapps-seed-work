// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace DrifterApps.Seeds.Tests.Scenarios;

public interface IStepRunner : IRunnerContext
{
    IStepRunner Execute(string message, Action action);
    IStepRunner Execute(string message, Func<Task> action);
}
