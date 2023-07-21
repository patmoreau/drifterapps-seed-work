// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace DrifterApps.Seeds.Testing.Scenarios;

public interface IStepRunner : IRunnerContext
{
    IStepRunner Execute(string description, Action stepExecution);
    IStepRunner Execute(string description, Func<Task> stepExecution);
}
