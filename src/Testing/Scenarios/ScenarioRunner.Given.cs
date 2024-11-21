namespace DrifterApps.Seeds.Testing.Scenarios;

internal sealed partial class ScenarioRunner
{
    /// <summary>
    ///     Adds a step to the scenario with the given action.
    /// </summary>
    /// <param name="step">The action to be executed as a step.</param>
    /// <returns>The current instance of <see cref="IScenarioRunner" />.</returns>
    public IScenarioRunner Given(Action<IStepRunner> step)
    {
        ArgumentNullException.ThrowIfNull(step);

        _stepCommand = nameof(Given);

        step.Invoke(this);

        return this;
    }

    /// <summary>
    ///     Adds a step to the scenario with the given description and action.
    /// </summary>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The action to be executed as a step.</param>
    /// <returns>The current instance of <see cref="IScenarioRunner" />.</returns>
    public IScenarioRunner Given(string description, Action step)
    {
        AddStep(nameof(Given), description, async _ =>
        {
            await Task.Run(step).ConfigureAwait(false);
            return null;
        });
        return this;
    }

    /// <summary>
    ///     Adds a step to the scenario with the given description and asynchronous function.
    /// </summary>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The asynchronous function to be executed as a step.</param>
    /// <returns>The current instance of <see cref="IScenarioRunner" />.</returns>
    public IScenarioRunner Given(string description, Func<Task> step)
    {
        AddStep(nameof(Given), description, async _ =>
        {
            await step().ConfigureAwait(false);
            return null;
        });

        return this;
    }

    /// <summary>
    ///     Adds a step to the scenario with the given description and action with a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The action to be executed as a step.</param>
    /// <returns>The current instance of <see cref="IScenarioRunner" />.</returns>
    public IScenarioRunner Given<T>(string description, Action<T> step)
    {
        AddStep(nameof(Given), description, async input =>
        {
            await Task.Run(() => step((T) input!)).ConfigureAwait(false);
            return null;
        });
        return this;
    }

    /// <summary>
    ///     Adds a step to the scenario with the given description and function.
    /// </summary>
    /// <typeparam name="T">The type of the return value of the function.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The function to be executed as a step.</param>
    /// <returns>The current instance of <see cref="IScenarioRunner" />.</returns>
    public IScenarioRunner Given<T>(string description, Func<T> step)
    {
        AddStep(nameof(Given), description, async _ => await Task.Run(step).ConfigureAwait(false));

        return this;
    }

    /// <summary>
    ///     Adds a step to the scenario with the given description and asynchronous function.
    /// </summary>
    /// <typeparam name="T">The type of the return value of the function.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The asynchronous function to be executed as a step.</param>
    /// <returns>The current instance of <see cref="IScenarioRunner" />.</returns>
    public IScenarioRunner Given<T>(string description, Func<Task<T>> step)
    {
        AddStep(nameof(Given), description, async _ => await step().ConfigureAwait(false));

        return this;
    }

    /// <summary>
    ///     Adds a step to the scenario with the given description and asynchronous function with a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The asynchronous function to be executed as a step.</param>
    /// <returns>The current instance of <see cref="IScenarioRunner" />.</returns>
    public IScenarioRunner Given<T>(string description, Func<T, Task> step)
    {
        AddStep(nameof(Given), description, async input =>
        {
            await step((T) input!).ConfigureAwait(false);
            return null;
        });

        return this;
    }

    /// <summary>
    ///     Adds a step to the scenario with the given description and function with a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <typeparam name="T2">The type of the return value of the function.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The function to be executed as a step.</param>
    /// <returns>The current instance of <see cref="IScenarioRunner" />.</returns>
    public IScenarioRunner Given<T, T2>(string description, Func<T, T2> step)
    {
        AddStep(nameof(Given), description,
            async input => await Task.Run(() => step((T) input!)).ConfigureAwait(false));

        return this;
    }

    /// <summary>
    ///     Adds a step to the scenario with the given description and asynchronous function with a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <typeparam name="T2">The type of the return value of the function.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The asynchronous function to be executed as a step.</param>
    /// <returns>The current instance of <see cref="IScenarioRunner" />.</returns>
    public IScenarioRunner Given<T, T2>(string description, Func<T, Task<T2>> step)
    {
        AddStep(nameof(Given), description, async input => await step((T) input!).ConfigureAwait(false));

        return this;
    }
}
