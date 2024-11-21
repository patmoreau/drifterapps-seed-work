using System.Diagnostics.CodeAnalysis;
using DrifterApps.Seeds.Testing.Attributes;
using Xunit.Abstractions;

namespace DrifterApps.Seeds.Testing.Scenarios;

/// <summary>
///     Represents a runner for executing test scenarios.
/// </summary>
[SuppressMessage("Style", "IDE0042:Deconstruct variable declaration")]
internal sealed partial class ScenarioRunner : IScenarioRunner, IStepRunner
{
    private readonly Dictionary<string, object> _context = [];
    private readonly List<(string Command, string Description, Func<object?, Task<object?>> Step)> _steps = [];
    private readonly ITestOutputHelper _testOutputHelper;
    private string _stepCommand = string.Empty;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScenarioRunner" /> class.
    /// </summary>
    /// <param name="description">The description of the scenario.</param>
    /// <param name="testOutputHelper">The test output helper.</param>
    /// <exception cref="ArgumentNullException">Thrown when description or testOutputHelper is null.</exception>
    private ScenarioRunner(string description, ITestOutputHelper testOutputHelper)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentNullException(nameof(description),
                "Please explain your intent by documenting your scenario.");
        }

        ArgumentNullException.ThrowIfNull(testOutputHelper);

        _steps.Add((Command: "Scenario", $"SCENARIO for {description}", _ => Task.FromResult<object?>(null)));

        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    ///     Sets context data for the scenario.
    /// </summary>
    /// <param name="contextKey">The key for the context data.</param>
    /// <param name="data">The data to be set.</param>
    public void SetContextData(string contextKey, object data)
    {
        _context.Remove(contextKey);
        _context.Add(contextKey, data);
    }

    /// <summary>
    ///     Gets context data for the scenario.
    /// </summary>
    /// <typeparam name="T">The type of the context data.</typeparam>
    /// <param name="contextKey">The key for the context data.</param>
    /// <returns>The context data.</returns>
    [AssertionMethod]
    public T GetContextData<T>(string contextKey)
    {
        _context.Should().ContainKey(contextKey);
        return (T) _context[contextKey];
    }

    /// <summary>
    ///     Executes a step in the scenario.
    /// </summary>
    /// <param name="description">The description of the step.</param>
    /// <param name="stepExecution">The action to be executed.</param>
    /// <returns>The step runner.</returns>
    public IStepRunner Execute(string description, Action stepExecution)
    {
        _ = _stepCommand switch
        {
            nameof(Given) => Given(description, stepExecution),
            nameof(When) => When(description, stepExecution),
            nameof(Then) => Then(description, stepExecution),
            _ => Given(description, stepExecution)
        };

        return this;
    }

    /// <summary>
    ///     Executes an asynchronous step in the scenario.
    /// </summary>
    /// <param name="description">The description of the step.</param>
    /// <param name="stepExecution">The function to be executed.</param>
    /// <returns>The step runner.</returns>
    public IStepRunner Execute(string description, Func<Task> stepExecution)
    {
        _ = _stepCommand switch
        {
            nameof(Given) => Given(description, stepExecution),
            nameof(When) => When(description, stepExecution),
            nameof(Then) => Then(description, stepExecution),
            _ => Given(description, stepExecution)
        };

        return this;
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="ScenarioRunner" /> class.
    /// </summary>
    /// <param name="description">The description of the scenario.</param>
    /// <param name="testOutputHelper">The test output helper.</param>
    /// <returns>A new instance of <see cref="ScenarioRunner" />.</returns>
    public static ScenarioRunner Create(string description, ITestOutputHelper testOutputHelper)
        => new(description, testOutputHelper);

    /// <summary>
    ///     Plays the scenario asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PlayAsync()
    {
        var steps = _steps.ToList();
        _steps.Clear();

        object? currentResult = null;

        foreach (var step in steps)
        {
            try
            {
                currentResult = await step.Step(currentResult).ConfigureAwait(false);
                _testOutputHelper.WriteLine($"\u2713 {step.Description}");
            }
            catch (Exception)
            {
                _testOutputHelper.WriteLine($"\u2717 {step.Description}");
                throw;
            }
        }
    }

    /// <summary>
    ///     Adds a step to the scenario.
    /// </summary>
    /// <param name="command">The command for the step.</param>
    /// <param name="description">The description of the step.</param>
    /// <param name="step">The function representing the step.</param>
    /// <exception cref="ArgumentNullException">Thrown when description is null or empty.</exception>
    private void AddStep(string command, string description, Func<object?, Task<object?>> step)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentNullException(nameof(description),
                "Please explain your intent by documenting your test.");
        }

        var previousCommand = _steps.LastOrDefault();
        var textCommand = command.Equals(previousCommand.Command, StringComparison.OrdinalIgnoreCase)
            ? "and"
            : command.ToUpperInvariant();
        var text = $"{textCommand} {description}";
        _steps.Add((command, $"{text}", step));
    }
}
