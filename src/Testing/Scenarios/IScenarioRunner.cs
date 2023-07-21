// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace DrifterApps.Seeds.Testing.Scenarios;

#pragma warning disable CA1716
public interface IScenarioRunner : IRunnerContext
{
    IScenarioRunner Given(string description, Action step);
    IScenarioRunner Given(string description, Func<Task> step);
    IScenarioRunner Given(Action<IStepRunner> step);
    IScenarioRunner When(string description, Action step);
    IScenarioRunner When(string description, Func<Task> step);
    IScenarioRunner When(Action<IStepRunner> step);
    IScenarioRunner Then(string description, Action step);
    IScenarioRunner Then(string description, Func<Task> step);
    IScenarioRunner Then(Action<IStepRunner> step);
    IScenarioRunner And(string description, Action step);
    IScenarioRunner And(string description, Func<Task> step);
    IScenarioRunner And(Action<IStepRunner> step);
}
