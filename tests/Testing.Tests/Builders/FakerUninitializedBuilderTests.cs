// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

using Bogus;
using FluentAssertions.Execution;

namespace DrifterApps.Seeds.Testing.Tests.Builders;

[UnitTest]
public class FakerUninitializedBuilderTests
{
    private const string DefaultPrivateInitProperty = "default_private_init_property";
    private const string DefaultNormalProperty = "default_normal_property";
    private const string DefaultInitProperty = "default_init_property";
    private const string DefaultInternalInitProperty = "default_internal_init_property";
    private static readonly Guid DefaultId = Guid.Parse("98306af0-721e-11ef-a499-0800200c9a66");

    [Fact]
    public void GivenFakeRecordBuilder_WhenBuildingWithBaseConfig_ShouldReturnARandomInstanceWithDefaultValues()
    {
        // Arrange
        var builder = new FakeUninitializedRecordBuilder();

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
    public void GivenFakeRecordBuilder_WhenBuildingWithId_ShouldReturnExpectedInstance()
    {
        // Arrange
        var builder = new FakeUninitializedRecordBuilder().WithNoId();

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
    public void GivenFakeRecordBuilder_WhenBuildingWithVisibleProperty_ShouldReturnExpectedInstance()
    {
        // Arrange
        var builder = new FakeUninitializedRecordBuilder().WithPrivateInitPropertyHasHash();

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

    private record FakeUninitializedRecord(
        Guid Id,
        string PrivateInitProperty,
        string NormalProperty,
        string InitProperty,
        string InternalInitProperty);

    private class FakeUninitializedRecordBuilder : FakerBuilder<FakeUninitializedRecord>
    {
        protected override Faker<FakeUninitializedRecord> Faker { get; } = CreateUninitializedFaker()
                .RuleFor(x => x.Id, DefaultId)
                .RuleFor(x => x.PrivateInitProperty, DefaultPrivateInitProperty)
                .RuleFor(x => x.NormalProperty, DefaultNormalProperty)
                .RuleFor(x => x.InitProperty, DefaultInitProperty)
                .RuleFor(x => x.InternalInitProperty, DefaultInternalInitProperty);

        public FakeUninitializedRecordBuilder WithNoId()
        {
            Faker.RuleFor(x => x.Id, _ => Guid.Empty);
            return this;
        }

        public FakeUninitializedRecordBuilder WithPrivateInitPropertyHasHash()
        {
            Faker.RuleFor(x => x.PrivateInitProperty, faker => faker.Random.Hash());
            return this;
        }
    }
}
