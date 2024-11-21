namespace DrifterApps.Seeds.Testing.Scenarios;

internal sealed partial class ScenarioRunner
{
    /// <summary>
    ///     Adds a step to the scenario with the specified action.
    /// </summary>
    /// <param name="step">The action to be executed as a step.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner And(Action<IStepRunner> step)
    {
        ArgumentNullException.ThrowIfNull(step);

        var previousCommand = _steps.LastOrDefault();

        return previousCommand.Command switch
        {
            nameof(Given) => Given(step),
            nameof(When) => When(step),
            nameof(Then) => Then(step),
            _ => Given(step)
        };
    }

    /// <summary>
    ///     Adds a step to the scenario with the specified description and action.
    /// </summary>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The action to be executed as a step.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner And(string description, Action step)
    {
        var previousCommand = _steps.LastOrDefault();

        return string.IsNullOrWhiteSpace(previousCommand.Command)
            ? Given(description, step)
            : previousCommand.Command switch
            {
                nameof(Given) => Given(description, step),
                nameof(When) => When(description, step),
                nameof(Then) => Then(description, step),
                _ => Given(description, step)
            };
    }

    /// <summary>
    ///     Adds a step to the scenario with the specified description and asynchronous function.
    /// </summary>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The asynchronous function to be executed as a step.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner And(string description, Func<Task> step)
    {
        var previousCommand = _steps.LastOrDefault();

        return previousCommand.Command switch
        {
            nameof(Given) => Given(description, step),
            nameof(When) => When(description, step),
            nameof(Then) => Then(description, step),
            _ => Given(description, step)
        };
    }

    /// <summary>
    ///     Adds a step to the scenario with the specified description and action with a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The action to be executed as a step.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner And<T>(string description, Action<T> step)
    {
        var previousCommand = _steps.LastOrDefault();

        return string.IsNullOrWhiteSpace(previousCommand.Command)
            ? Given(description, step)
            : previousCommand.Command switch
            {
                nameof(Given) => Given(description, step),
                nameof(When) => When(description, step),
                nameof(Then) => Then(description, step),
                _ => Given(description, step)
            };
    }

    /// <summary>
    ///     Adds a step to the scenario with the specified description and function.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The function to be executed as a step.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner And<T>(string description, Func<T> step)
    {
        var previousCommand = _steps.LastOrDefault();

        return previousCommand.Command switch
        {
            nameof(Given) => Given(description, step),
            nameof(When) => When(description, step),
            nameof(Then) => Then(description, step),
            _ => Given(description, step)
        };
    }

    /// <summary>
    ///     Adds a step to the scenario with the specified description and asynchronous function with a return type.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The asynchronous function to be executed as a step.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner And<T>(string description, Func<Task<T>> step)
    {
        var previousCommand = _steps.LastOrDefault();

        return previousCommand.Command switch
        {
            nameof(Given) => Given(description, step),
            nameof(When) => When(description, step),
            nameof(Then) => Then(description, step),
            _ => Given(description, step)
        };
    }

    /// <summary>
    ///     Adds a step to the scenario with the specified description and function with two parameters.
    /// </summary>
    /// <typeparam name="T">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The function to be executed as a step.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner And<T, T2>(string description, Func<T, T2> step)
    {
        var previousCommand = _steps.LastOrDefault();

        return previousCommand.Command switch
        {
            nameof(Given) => Given(description, step),
            nameof(When) => When(description, step),
            nameof(Then) => Then(description, step),
            _ => Given(description, step)
        };
    }

    /// <summary>
    ///     Adds a step to the scenario with the specified description and asynchronous function with two parameters.
    /// </summary>
    /// <typeparam name="T">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The return type of the function.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The asynchronous function to be executed as a step.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner And<T, T2>(string description, Func<T, Task<T2>> step)
    {
        var previousCommand = _steps.LastOrDefault();

        return previousCommand.Command switch
        {
            nameof(Given) => Given(description, step),
            nameof(When) => When(description, step),
            nameof(Then) => Then(description, step),
            _ => Given(description, step)
        };
    }

    /// <summary>
    ///     Adds a step to the scenario with the specified description and asynchronous function with a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The asynchronous function to be executed as a step.</param>
    /// <returns>The scenario runner instance.</returns>
    public IScenarioRunner And<T>(string description, Func<T, Task> step)
    {
        var previousCommand = _steps.LastOrDefault();

        return previousCommand.Command switch
        {
            nameof(Given) => Given(description, step),
            nameof(When) => When(description, step),
            nameof(Then) => Then(description, step),
            _ => Given(description, step)
        };
    }
}
