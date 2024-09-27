// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

using System.Collections.Immutable;

namespace DrifterApps.Seeds.Testing.Tests.Builders;

[UnitTest]
public class FakerPrivateBuilderTests
{
    private const string DefaultPrivateInitProperty = "default_private_init_property";
    private const string DefaultNormalProperty = "default_normal_property";
    private const string DefaultInitProperty = "default_init_property";
    private const string DefaultInternalInitProperty = "default_internal_init_property";
    private static readonly Guid DefaultId = Guid.Parse("98306af0-721e-11ef-a499-0800200c9a66");

    [Fact]
    public void GivenFakePrivateClassBuilder_WhenBuildingWithBaseConfig_ShouldReturnARandomInstanceWithDefaultValues()
    {
        // Arrange
        var builder = FakerBuilder<FakePrivateClass>.CreatePrivateBuilder<FakeClassPrivateBuilder>();

        // Act
        var result = builder.Build();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(DefaultId);
        result.PrivateInitProperty.Should().Be(DefaultPrivateInitProperty);
        result.NormalProperty.Should().Be(DefaultNormalProperty);
        result.InitProperty.Should().Be(DefaultInitProperty);
        result.InternalInitProperty.Should().Be(DefaultInternalInitProperty);
        result.ReadOnlyCollection.Should().BeEmpty();
        result.PrivateDecimalField.Should().Be(20);
    }

    [Fact]
    public void GivenFakeClassPrivateBuilder_WhenBuildingWithId_ShouldReturnExpectedInstance()
    {
        // Arrange
        var builder = FakerBuilder<FakePrivateClass>.CreatePrivateBuilder<FakeClassPrivateBuilder>().WithNoId();

        // Act
        var result = builder.Build();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(Guid.Empty);
        result.PrivateInitProperty.Should().Be(DefaultPrivateInitProperty);
        result.NormalProperty.Should().Be(DefaultNormalProperty);
        result.InitProperty.Should().Be(DefaultInitProperty);
        result.InternalInitProperty.Should().Be(DefaultInternalInitProperty);
    }

    [Fact]
    public void GivenFakeClassPrivateBuilder_WhenBuildingWithVisibleProperty_ShouldReturnExpectedInstance()
    {
        // Arrange
        var builder = FakerBuilder<FakePrivateClass>.CreatePrivateBuilder<FakeClassPrivateBuilder>()
            .WithPrivateInitPropertyHasHash();

        // Act
        var result = builder.Build();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(DefaultId);
        result.PrivateInitProperty.Should().NotBe(DefaultPrivateInitProperty);
        result.NormalProperty.Should().Be(DefaultNormalProperty);
        result.InitProperty.Should().Be(DefaultInitProperty);
        result.InternalInitProperty.Should().Be(DefaultInternalInitProperty);
    }

    private class FakePrivateClass
    {
        private readonly decimal _privateField = 0;
        private readonly IReadOnlyCollection<string> _readOnlyCollection = [];

        private FakePrivateClass(Guid id) => Id = id;

        public Guid Id { get; }

        public int NeverReplaceProperty => CreateValue();

        public string PrivateInitProperty { get; } = string.Empty;

        public string NormalProperty { get; } = string.Empty;

        public string InitProperty { get; } = string.Empty;

        public string InternalInitProperty { get; } = string.Empty;

        public IReadOnlyCollection<string> ReadOnlyCollection => _readOnlyCollection.ToImmutableList();

        public decimal PrivateDecimalField => _privateField + 10;

        private int CreateValue() => PrivateInitProperty.Length;

        public static FakePrivateClass Create() => new(Guid.NewGuid());
    }

    private class FakeClassPrivateBuilder : FakerBuilder<FakePrivateClass>
    {
        protected override void ConfigureFakerRules() =>
            Faker
                .RuleFor(x => x.Id, DefaultId)
                .RuleFor(x => x.PrivateInitProperty, DefaultPrivateInitProperty)
                .RuleFor(x => x.NormalProperty, DefaultNormalProperty)
                .RuleFor(x => x.InitProperty, DefaultInitProperty)
                .RuleFor(x => x.InternalInitProperty, DefaultInternalInitProperty)
                .RuleFor(x => x.ReadOnlyCollection, _ => [])
                .RuleFor("PrivateField", _ => 10m);

        public FakeClassPrivateBuilder WithNoId()
        {
            Faker.RuleFor(x => x.Id, _ => Guid.Empty);
            return this;
        }

        public FakeClassPrivateBuilder WithPrivateInitPropertyHasHash()
        {
            Faker.RuleFor(x => x.PrivateInitProperty, faker => faker.Random.Hash());
            return this;
        }
    }
}
