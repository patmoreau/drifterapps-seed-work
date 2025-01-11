using Bogus;

namespace DrifterApps.Seeds.Testing.Tests.Fakes;

internal class FakeClass
{
    public required string Name { get; init; }

    public int Age { get; init; }
}

internal class FakeClassBuilder : FakerBuilder<FakeClass>
{
    protected override Faker<FakeClass> Faker { get; } = CreateFaker<FakeClass>()
        .RuleFor(x => x.Name, f => f.Name.FullName())
        .RuleFor(x => x.Age, f => f.Random.Number(18, 65));
}
