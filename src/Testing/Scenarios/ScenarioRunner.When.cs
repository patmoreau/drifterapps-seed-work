namespace DrifterApps.Seeds.Testing.Scenarios;

internal sealed partial class ScenarioRunner
{
    /// <summary>
    ///     Executes a step in the scenario.
    /// </summary>
    /// <param name="step">The step to execute.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner When(Action<IStepRunner> step)
    {
        ArgumentNullException.ThrowIfNull(step);

        _stepCommand = nameof(When);

        step.Invoke(this);

        return this;
    }

    /// <summary>
    ///     Executes a step in the scenario with a description.
    /// </summary>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The step to execute.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner When(string description, Action step)
    {
        AddStep(nameof(When), description, async _ =>
        {
            await Task.Run(step).ConfigureAwait(false);
            return null;
        });

        return this;
    }

    /// <summary>
    ///     Executes an asynchronous step in the scenario with a description.
    /// </summary>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The asynchronous step to execute.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner When(string description, Func<Task> step)
    {
        AddStep(nameof(When), description, async _ =>
        {
            await step().ConfigureAwait(false);
            return null;
        });

        return this;
    }

    /// <summary>
    ///     Executes a step in the scenario with a description and input parameter.
    /// </summary>
    /// <typeparam name="T">The type of the input parameter.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The step to execute.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner When<T>(string description, Action<T> step)
    {
        AddStep(nameof(When), description, async input =>
        {
            await Task.Run(() => step((T) input!)).ConfigureAwait(false);
            return null;
        });

        return this;
    }

    /// <summary>
    ///     Executes a step in the scenario with a description and returns a result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The step to execute.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner When<T>(string description, Func<T> step)
    {
        AddStep(nameof(When), description, async _ => await Task.Run(step).ConfigureAwait(false));

        return this;
    }

    /// <summary>
    ///     Executes an asynchronous step in the scenario with a description and returns a result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The asynchronous step to execute.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner When<T>(string description, Func<Task<T>> step)
    {
        AddStep(nameof(When), description, async _ => await step().ConfigureAwait(false));

        return this;
    }

    /// <summary>
    ///     Executes a step in the scenario with a description and input parameter, and returns a result.
    /// </summary>
    /// <typeparam name="T">The type of the input parameter.</typeparam>
    /// <typeparam name="T2">The type of the result.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The step to execute.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner When<T, T2>(string description, Func<T, T2> step)
    {
        AddStep(nameof(When), description, async input => await Task.Run(() => step((T) input!)).ConfigureAwait(false));

        return this;
    }

    /// <summary>
    ///     Executes an asynchronous step in the scenario with a description and input parameter, and returns a result.
    /// </summary>
    /// <typeparam name="T">The type of the input parameter.</typeparam>
    /// <typeparam name="T2">The type of the result.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The asynchronous step to execute.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner When<T, T2>(string description, Func<T, Task<T2>> step)
    {
        AddStep(nameof(When), description, async input => await step((T) input!).ConfigureAwait(false));

        return this;
    }

    /// <summary>
    ///     Executes an asynchronous step in the scenario with a description and input parameter.
    /// </summary>
    /// <typeparam name="T">The type of the input parameter.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The asynchronous step to execute.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner When<T>(string description, Func<T, Task> step)
    {
        AddStep(nameof(When), description, async input =>
        {
            await step((T) input!).ConfigureAwait(false);
            return null;
        });

        return this;
    }
}
