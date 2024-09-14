// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace DrifterApps.Seeds.Testing.Tests.Builders;

[UnitTest]
public class FakerBuilderTests
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

    [Fact]
    public void GivenFakeClassBuilder_WhenBuildingWithBaseConfig_ShouldReturnARandomInstanceWithDefaultValues()
    {
        // Arrange
        var builder = FakerBuilder<FakeClass>.CreateBuilder<FakeClassBuilder>();

        // Act
        var result = builder.Build();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(DefaultId);
        result.PrivateInitProperty.Should().Be(DefaultPrivateInitProperty);
        result.NormalProperty.Should().Be(DefaultNormalProperty);
        result.InitProperty.Should().Be(DefaultInitProperty);
        result.InternalInitProperty.Should().Be(DefaultInternalInitProperty);
    }

    [Fact]
    public void GivenFakeClassBuilder_WhenBuildingWithId_ShouldReturnExpectedInstance()
    {
        // Arrange
        var builder = FakerBuilder<FakeClass>.CreateBuilder<FakeClassBuilder>().WithNoId();

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
    public void GivenFakeClassBuilder_WhenBuildingWithVisibleProperty_ShouldReturnExpectedInstance()
    {
        // Arrange
        var builder = FakerBuilder<FakeClass>.CreateBuilder<FakeClassBuilder>().WithPrivateInitPropertyHasHash();

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

    [Fact]
    public void GivenFakeRecordBuilder_WhenBuildingWithBaseConfig_ShouldReturnARandomInstanceWithDefaultValues()
    {
        // Arrange
        var builder = FakerBuilder<FakeRecord>.CreateRecordBuilder<FakeRecordBuilder>();

        // Act
        var result = builder.Build();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(DefaultId);
        result.PrivateInitProperty.Should().Be(DefaultPrivateInitProperty);
        result.NormalProperty.Should().Be(DefaultNormalProperty);
        result.InitProperty.Should().Be(DefaultInitProperty);
        result.InternalInitProperty.Should().Be(DefaultInternalInitProperty);
    }

    [Fact]
    public void GivenFakeRecordBuilder_WhenBuildingWithId_ShouldReturnExpectedInstance()
    {
        // Arrange
        var builder = FakerBuilder<FakeRecord>.CreateRecordBuilder<FakeRecordBuilder>().WithNoId();

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
    public void GivenFakeRecordBuilder_WhenBuildingWithVisibleProperty_ShouldReturnExpectedInstance()
    {
        // Arrange
        var builder = FakerBuilder<FakeRecord>.CreateRecordBuilder<FakeRecordBuilder>()
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

    private class FakeClassPrivateBuilder : FakerBuilder<FakePrivateClass>
    {
        protected override void ConfigureFakerRules() =>
            Faker
                .RuleFor(x => x.Id, DefaultId)
                .RuleFor(x => x.PrivateInitProperty, DefaultPrivateInitProperty)
                .RuleFor(x => x.NormalProperty, DefaultNormalProperty)
                .RuleFor(x => x.InitProperty, DefaultInitProperty)
                .RuleFor(x => x.InternalInitProperty, DefaultInternalInitProperty);

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

    private class FakeClassBuilder : FakerBuilder<FakeClass>
    {
        protected override void ConfigureFakerRules() =>
            Faker
                .RuleFor(x => x.Id, DefaultId)
                .RuleFor(x => x.PrivateInitProperty, DefaultPrivateInitProperty)
                .RuleFor(x => x.NormalProperty, DefaultNormalProperty)
                .RuleFor(x => x.InitProperty, DefaultInitProperty)
                .RuleFor(x => x.InternalInitProperty, DefaultInternalInitProperty);

        public FakeClassBuilder WithNoId()
        {
            Faker.RuleFor(x => x.Id, _ => Guid.Empty);
            return this;
        }

        public FakeClassBuilder WithPrivateInitPropertyHasHash()
        {
            Faker.RuleFor(x => x.PrivateInitProperty, faker => faker.Random.Hash());
            return this;
        }
    }

    private class FakeRecordBuilder : FakerBuilder<FakeRecord>
    {
        protected override void ConfigureFakerRules() =>
            Faker
                .RuleFor(x => x.Id, DefaultId)
                .RuleFor(x => x.PrivateInitProperty, DefaultPrivateInitProperty)
                .RuleFor(x => x.NormalProperty, DefaultNormalProperty)
                .RuleFor(x => x.InitProperty, DefaultInitProperty)
                .RuleFor(x => x.InternalInitProperty, DefaultInternalInitProperty);

        public FakeRecordBuilder WithNoId()
        {
            Faker.RuleFor(x => x.Id, _ => Guid.Empty);
            return this;
        }

        public FakeRecordBuilder WithPrivateInitPropertyHasHash()
        {
            Faker.RuleFor(x => x.PrivateInitProperty, faker => faker.Random.Hash());
            return this;
        }
    }

    private class FakePrivateClass
    {
        private FakePrivateClass(Guid id) => Id = id;

        public Guid Id { get; }

        public int NeverReplaceProperty => CreateValue();

        public string PrivateInitProperty { get; } = string.Empty;

        public string NormalProperty { get; } = string.Empty;

        public string InitProperty { get; } = string.Empty;

        public string InternalInitProperty { get; } = string.Empty;

        private int CreateValue() => PrivateInitProperty.Length;

        public static FakePrivateClass Create() => new(Guid.NewGuid());
    }

    private class FakeClass
    {
        public Guid Id { get; init; }

        public int NeverReplaceProperty => CreateValue();

        public string PrivateInitProperty { get; init; } = string.Empty;

        public string NormalProperty { get; init; } = string.Empty;

        public string InitProperty { get; init; } = string.Empty;

        public string InternalInitProperty { get; init; } = string.Empty;

        private int CreateValue() => PrivateInitProperty.Length;
    }

    private record FakeRecord(
        Guid Id,
        string PrivateInitProperty,
        string NormalProperty,
        string InitProperty,
        string InternalInitProperty);
}
