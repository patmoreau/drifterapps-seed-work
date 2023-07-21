using DrifterApps.Seeds.Testing.Attributes;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Abstractions;

namespace DrifterApps.Seeds.Testing.Scenarios;

internal sealed class ScenarioRunner : IScenarioRunner, IStepRunner
{
    private readonly IDictionary<string, object> _context = new Dictionary<string, object>();
    private readonly List<(string Command, string Description, Func<Task> Step)> _steps = new();
    private readonly ITestOutputHelper _testOutputHelper;
    private string _stepCommand = string.Empty;

    private ScenarioRunner(string description, ITestOutputHelper testOutputHelper)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentNullException(nameof(description),
                "Please explain your intent by documenting your scenario.");

        ArgumentNullException.ThrowIfNull(testOutputHelper);

        _steps.Add((Command: "Scenario", $"SCENARIO for {description}", () => Task.CompletedTask));

        _testOutputHelper = testOutputHelper;
    }

    public IScenarioRunner Given(string description, Action step)
    {
        AddStep(nameof(Given), description, step);
        return this;
    }

    public IScenarioRunner Given(string description, Func<Task> step)
    {
        AddStep(nameof(Given), description, step);

        return this;
    }

    public IScenarioRunner Given(Action<IStepRunner> step)
    {
        ArgumentNullException.ThrowIfNull(step);

        _stepCommand = nameof(Given);

        step.Invoke(this);

        return this;
    }

    public IScenarioRunner When(string description, Action step)
    {
        AddStep(nameof(When), description, step);

        return this;
    }

    public IScenarioRunner When(string description, Func<Task> step)
    {
        AddStep(nameof(When), description, step);

        return this;
    }

    public IScenarioRunner When(Action<IStepRunner> step)
    {
        ArgumentNullException.ThrowIfNull(step);

        _stepCommand = nameof(When);

        step.Invoke(this);

        return this;
    }

    public IScenarioRunner Then(string description, Action step)
    {
        AddStep(nameof(Then), description, async () =>
        {
            using AssertionScope scope = new();
            await Task.Run(step).ConfigureAwait(false);
        });

        return this;
    }

    public IScenarioRunner Then(string description, Func<Task> step)
    {
        AddStep(nameof(Then), description, async () =>
        {
            using AssertionScope scope = new();
            await step().ConfigureAwait(false);
        });

        return this;
    }

    public IScenarioRunner Then(Action<IStepRunner> step)
    {
        ArgumentNullException.ThrowIfNull(step);

        _stepCommand = nameof(Then);

        step.Invoke(this);

        return this;
    }

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

    public IScenarioRunner And(string description, Action step)
    {
        var previousCommand = _steps.LastOrDefault();

        if (string.IsNullOrWhiteSpace(previousCommand.Command)) return Given(description, step);

        return previousCommand.Command switch
        {
            nameof(Given) => Given(description, step),
            nameof(When) => When(description, step),
            nameof(Then) => Then(description, step),
            _ => Given(description, step)
        };
    }

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

    public void SetContextData(string contextKey, object data)
    {
        if (_context.ContainsKey(contextKey)) _context.Remove(contextKey);

        _context.Add(contextKey, data);
    }

    [AssertionMethod]
    public T GetContextData<T>(string contextKey)
    {
        _context.Should().ContainKey(contextKey);
        return (T) _context[contextKey];
    }

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

    public static ScenarioRunner Create(string description, ITestOutputHelper testOutputHelper)
        => new(description, testOutputHelper);

    public async Task PlayAsync()
    {
        var steps = _steps.ToList();
        _steps.Clear();

        foreach (var step in steps)
            try
            {
                await step.Step().ConfigureAwait(false);
                _testOutputHelper.WriteLine($"\u2713 {step.Description}");
            }
            catch (Exception)
            {
                _testOutputHelper.WriteLine($"\u2717 {step.Description}");
                throw;
            }
    }

    private void AddStep(string command, string description, Action step) =>
        AddStep(command, description, async () => await Task.Run(step).ConfigureAwait(false));

    private void AddStep(string command, string description, Func<Task> step)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentNullException(nameof(description),
                "Please explain your intent by documenting your test.");

        var previousCommand = _steps.LastOrDefault();
        var textCommand = command.Equals(previousCommand.Command, StringComparison.OrdinalIgnoreCase)
            ? "and"
            : command.ToUpperInvariant();
        var text = $"{textCommand} {description}";
        _steps.Add((command, $"{text}", async () => await step().ConfigureAwait(false)));
    }
}
