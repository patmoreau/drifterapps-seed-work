using Bogus;
using DrifterApps.Seeds.FluentResult;
using DrifterApps.Seeds.Testing;

namespace DrifterApps.Seeds.Application.Tests;

[UnitTest]
public class QueryParamsTests
{
    private readonly FakeRequestBuilder _builder = new();

    [Fact]
    public void GivenCreate_WhenRequestQueryIsNull_ThenReturnFailure()
    {
        // Arrange
        IRequestQuery requestQuery = null!;

        // Act
        var result = QueryParams.Create(requestQuery);

        // Assert
        result.Should().BeFailure()
            .And.WithError(QueryParamsErrors.RequestIsRequired);
    }

    [Fact]
    public void GivenCreate_WhenOffsetIsNegative_ThenReturnFailure()
    {
        // Arrange
        var requestQuery = _builder.WithNegativeOffset().Build();

        // Act
        var result = QueryParams.Create(requestQuery);

        // Assert
        result.Should().BeFailure().And
            .WithError(CreateErrorAggregate(QueryParamsErrors.OffsetCannotBeNegative));
    }

    private static ResultErrorAggregate CreateErrorAggregate(ResultError error) =>
        new($"{nameof(QueryParams)}.Errors",
            "Errors occurred",
            new Dictionary<string, string[]>
            {
                {error.Code, [error.Description]}
            });

    [Fact]
    public void GivenCreate_WhenLimitIsNotPositive_ThenReturnFailure()
    {
        // Arrange
        var requestQuery = _builder.WithNotPositiveLimit().Build();

        // Act
        var result = QueryParams.Create(requestQuery);

        // Assert
        result.Should().BeFailure()
            .And.WithError(CreateErrorAggregate(QueryParamsErrors.LimitMustBePositive));
    }

    [Theory]
    [InlineData("")]
    [InlineData("+invalid")]
    [InlineData("invalid-field")]
    [InlineData("-field-invalid")]
    public void GivenCreate_WhenSortIsInvalid_ThenReturnFailure(string sort)
    {
        // Arrange
        var requestQuery = _builder.WithSortPattern(sort).Build();

        // Act
        var result = QueryParams.Create(requestQuery);

        // Assert
        result.Should().BeFailure()
            .And.WithError(CreateErrorAggregate(QueryParamsErrors.SortInvalidPattern(sort)));
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    [InlineData("field:invalid:value")]
    [InlineData("field:eq:")]
    public void GivenCreate_WhenFilterIsInvalid_ThenReturnFailure(string filter)
    {
        // Arrange
        var requestQuery = _builder.WithFilterPattern(filter).Build();

        // Act
        var result = QueryParams.Create(requestQuery);

        // Assert
        result.Should().BeFailure()
            .And.WithError(CreateErrorAggregate(QueryParamsErrors.FilterInvalidPattern(filter)));
    }

    [Fact]
    public void GivenCreate_WhenUsingDefaultValues_ThenReturnEmpty()
    {
        // Arrange
        var requestQuery = _builder.WithDefaultValues().Build();

        // Act
        var result = QueryParams.Create(requestQuery);

        // Assert
        result.Should().BeSuccessful().And.WithValue(QueryParams.Empty);
    }

    [Fact]
    public void GivenGetHashCode_WhenUsingEquivalentValues_ThenEquals()
    {
        // Arrange
        var requestQuery = _builder.Build();
        var query = QueryParams.Create(requestQuery).Value;

        // Act
        var result = query.GetHashCode();

        // Assert
        result.Should().Be(_builder.WhenExpectedHashCode(requestQuery));
    }

    [Fact]
    public void GivenEqualOp_WhenUsingEquivalentValues_ThenEquals()
    {
        // Arrange
        var requestQuery = _builder.WithDefaultValues().Build();
        var query = QueryParams.Create(requestQuery).Value;

        // Act
        var result = query == QueryParams.Empty;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GivenNotEqualOp_WhenNotUsingEquivalentValues_ThenNotEquals()
    {
        // Arrange
        var requestQuery = _builder.Build();
        var query = QueryParams.Create(requestQuery).Value;

        // Act
        var result = query != QueryParams.Empty;

        // Assert
        result.Should().BeTrue();
    }

    private class FakeRequestBuilder : FakerBuilder<FakeRequest>
    {
        protected override Faker<FakeRequest> Faker { get; } = CreateUninitializedFaker()
            .RuleFor(x => x.Offset, faker => faker.Random.Int(0))
            .RuleFor(x => x.Limit, faker => faker.Random.Int(1))
            .RuleFor(x => x.Sort, faker => faker.Random.ArrayElements(["-field", "field"], 1))
            .RuleFor(x => x.Filter, faker => faker.Random.ArrayElements(["field:eq:value"], 1));

        public FakeRequestBuilder WithNegativeOffset()
        {
            Faker.RuleFor(x => x.Offset, faker => faker.Random.Int(max: -1));
            return this;
        }

        public FakeRequestBuilder WithNotPositiveLimit()
        {
            Faker.RuleFor(x => x.Limit, faker => faker.Random.Int(max: 0));
            return this;
        }

        public FakeRequestBuilder WithSortPattern(params string[] sorts)
        {
            Faker.RuleFor(x => x.Sort, sorts);
            return this;
        }

        public FakeRequestBuilder WithFilterPattern(params string[] filters)
        {
            Faker.RuleFor(x => x.Filter, filters);
            return this;
        }

        public FakeRequestBuilder WithDefaultValues()
        {
            Faker.RuleFor(x => x.Offset, QueryParams.DefaultOffset);
            Faker.RuleFor(x => x.Limit, QueryParams.DefaultLimit);
            Faker.RuleFor(x => x.Sort, [.. QueryParams.DefaultSort]);
            Faker.RuleFor(x => x.Filter, [.. QueryParams.DefaultFilter]);
            return this;
        }

#pragma warning disable CA1822
#pragma warning disable S2325
        public int WhenExpectedHashCode(IRequestQuery request)
#pragma warning restore S2325
#pragma warning restore CA1822
        {
            var hash = new HashCode();
            hash.Add(request.Offset);
            hash.Add(request.Limit);
            foreach (var item in request.Sort)
            {
                hash.Add(item);
            }

            foreach (var item in request.Filter)
            {
                hash.Add(item);
            }

            return hash.ToHashCode();
        }
    }

    private record FakeRequest(int Offset, int Limit, string[] Sort, string[] Filter) : IRequestQuery;
}
