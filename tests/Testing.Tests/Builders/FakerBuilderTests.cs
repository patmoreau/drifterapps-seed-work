// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

using Bogus;
using FluentAssertions.Execution;

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
    public void GivenFakeClassBuilder_WhenBuildingWithBaseConfig_ShouldReturnARandomInstanceWithDefaultValues()
    {
        // Arrange
        var builder = new FakeClassBuilder();

        // Act
        var result = builder.Build();

        // Assert
        using var scope = new AssertionScope();
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
        var builder = new FakeClassBuilder().WithNoId();

        // Act
        var result = builder.Build();

        // Assert
        using var scope = new AssertionScope();
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
        var builder = new FakeClassBuilder().WithPrivateInitPropertyHasHash();

        // Act
        var result = builder.Build();

        // Assert
        using var scope = new AssertionScope();
        result.Should().NotBeNull();
        result.Id.Should().Be(DefaultId);
        result.PrivateInitProperty.Should().NotBe(DefaultPrivateInitProperty);
        result.NormalProperty.Should().Be(DefaultNormalProperty);
        result.InitProperty.Should().Be(DefaultInitProperty);
        result.InternalInitProperty.Should().Be(DefaultInternalInitProperty);
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

    private class FakeClassBuilder : FakerBuilder<FakeClass>
    {
        protected override Faker<FakeClass> Faker { get; } = CreateFaker()
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
}
