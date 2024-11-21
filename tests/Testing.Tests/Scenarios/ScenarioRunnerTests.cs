using System.Diagnostics.CodeAnalysis;
using Bogus;
using DrifterApps.Seeds.Testing.Scenarios;
using Xunit.Abstractions;

namespace DrifterApps.Seeds.Testing.Tests.Scenarios;

[UnitTest]
[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
public class ScenarioRunnerTests(ITestOutputHelper testOutputHelper)
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task GivenAction_WhenRunAsync_ThenShouldRunSteps()
    {
        var sut = ScenarioRunner.Create("Testing Action Scenario", testOutputHelper)
            .Given("Task 1: Given Action", () => { })
            .And("Task 2: And Action", () => { })
            .When("Task 3: When Action", () => { })
            .And("Task 4: And Action", () => { })
            .Then("Task 5: Then Action", () => { true.Should().BeTrue(); })
            .And("Task 6: And Action", () => { true.Should().BeTrue(); });

        await ((ScenarioRunner) sut).PlayAsync();
    }

    [Fact]
    public async Task GivenFuncOfT_WhenRunAsync_ThenShouldRunSteps()
    {
        var initial = _faker.Random.Int();
        var sut = ScenarioRunner.Create("Testing Action Scenario", testOutputHelper)
            .Given("Task 0: Given Func<T>", () => initial)
            .Given("Task 1: Given Func<T>", (int number) => number + 10)
            .And("Task 2: And Func<T>", (int number) => number + 10)
            .When("Task 3: When Func<T>", (int number) => number + 10)
            .And("Task 4: And Func<T>", (int number) => number + 10)
            .Then("Task 5: Then Func<T>", (int number) =>
            {
                number.Should().Be(initial + 40);
                return number + 10;
            })
            .And("Task 6: And Func<T>", (int number) =>
            {
                number.Should().Be(initial + 50);
                return number + 10;
            });

        await ((ScenarioRunner) sut).PlayAsync();
    }
}
