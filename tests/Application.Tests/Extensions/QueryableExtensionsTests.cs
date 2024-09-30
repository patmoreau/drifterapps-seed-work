using Bogus;
using DrifterApps.Seeds.Application.Extensions;
using DrifterApps.Seeds.Domain;

namespace DrifterApps.Seeds.Application.Tests.Extensions;

[UnitTest]
public class QueryableExtensionsTests
{
    private static Faker<FakeClass> Faker => new Faker<FakeClass>()
        .CustomInstantiator(f => new FakeClass(MyId.New, f.Name.FullName(),
            f.Finance.Amount(), f.Random.Bool()))
        .Ignore(x => x.PersonId)
        .Ignore(x => x.Name)
        .Ignore(x => x.Bonus)
        .Ignore(x => x.IsWorking);

    [Fact]
    public void GivenQuery_WhenFilterDecimal_ThenReturnQuery()
    {
        // Arrange
        var data = Faker.Generate(5).AsQueryable();
        var person = data.Skip(2).Take(1).First();
        var filter = $"Bonus:eq:{person.Bonus}";
        var query = data.Query(QueryParams.Create(QueryParams.DefaultOffset, QueryParams.DefaultLimit,
            QueryParams.DefaultSort, [filter]).Value);

        // Act
        var result = query.ToList();

        // Assert
        result.Should().ContainSingle().Which.Should().Be(person);
    }

    [Fact]
    public void GivenQuery_WhenFilterString_ThenReturnQuery()
    {
        // Arrange
        var data = Faker.Generate(5).AsQueryable();
        var person = data.Skip(2).Take(1).First();
        var filter = $"Name:eq:{person.Name}";

        // Act
        var query = data.Query(QueryParams.Create(QueryParams.DefaultOffset, QueryParams.DefaultLimit,
            QueryParams.DefaultSort, [filter]).Value);
        var result = query.ToList();

        // Assert
        result.Should().ContainSingle().Which.Should().Be(person);
    }

    [Fact]
    public void GivenQuery_WhenFilterIPrimitiveType_ThenReturnQuery()
    {
        // Arrange
        var data = Faker.Generate(5).AsQueryable();
        var person = data.Skip(2).Take(1).First();
        var filter = $"PersonId:eq:{person.PersonId.Value}";

        // Act
        var query = data.Query(QueryParams.Create(QueryParams.DefaultOffset, QueryParams.DefaultLimit,
            QueryParams.DefaultSort, [filter]).Value);
        var result = query.ToList();

        // Assert
        result.Should().ContainSingle().Which.Should().Be(person);
    }

    [Fact]
    public void GivenQuery_WhenFilterBool_ThenReturnQuery()
    {
        // Arrange
        var data = Faker.Generate(5).AsQueryable();
        var person = data.Skip(2).Take(1).First();
        var isWorking = person.IsWorking ? "true" : "false";
        var filter = $"IsWorking:eq:{isWorking}";

        // Act
        var query = data.Query(QueryParams.Create(QueryParams.DefaultOffset, QueryParams.DefaultLimit,
            QueryParams.DefaultSort, [filter]).Value);
        var result = query.ToList();

        // Assert
        result.Should().Contain(person);
    }

    [Fact]
    public void GivenQuery_WhenSortDecimal_ThenReturnQuery()
    {
        // Arrange
        var data = Faker.Generate(5).AsQueryable();
        const string sort = "Bonus";

        // Act
        var query = data.Query(QueryParams.Create(QueryParams.DefaultOffset, QueryParams.DefaultLimit,
            [sort], QueryParams.DefaultFilter).Value);
        var result = query.ToList();

        // Assert
        result.Should().ContainInOrder(data.OrderBy(x => x.Bonus));
    }

    [Fact]
    public void GivenQuery_WhenSortStringDesc_ThenReturnQuery()
    {
        // Arrange
        var data = Faker.Generate(5).AsQueryable();
        const string sort = "-Name";

        // Act
        var query = data.Query(QueryParams.Create(QueryParams.DefaultOffset, QueryParams.DefaultLimit,
            [sort], QueryParams.DefaultFilter).Value);
        var result = query.ToList();

        // Assert
        result.Should().ContainInOrder(data.OrderByDescending(x => x.Name));
    }

    [Fact]
    public void GivenQuery_WhenSortIPrimitiveType_ThenReturnQuery()
    {
        // Arrange
        var data = Faker.Generate(5).AsQueryable();
        const string sort = "PersonId";

        // Act
        var query = data.Query(QueryParams.Create(QueryParams.DefaultOffset, QueryParams.DefaultLimit,
            [sort], QueryParams.DefaultFilter).Value);
        var result = query.ToList();

        // Assert
        result.Should().ContainInOrder(data.OrderBy(x => x.PersonId.Value));
    }

    [Fact]
    public void GivenQuery_WhenSortBoolDesc_ThenReturnQuery()
    {
        // Arrange
        var data = Faker.Generate(5).AsQueryable();
        var person = data.Skip(2).Take(1).First();
        const string sort = "-IsWorking";

        // Act
        var query = data.Query(QueryParams.Create(QueryParams.DefaultOffset, QueryParams.DefaultLimit,
            [sort], QueryParams.DefaultFilter).Value);
        var result = query.ToList();

        // Assert
        result.Should().Contain(person);
    }

    [Fact]
    public void GivenQuery_WhenOffset_ThenReturnQuery()
    {
        // Arrange
        const int offset = 2;
        var data = Faker.Generate(5).AsQueryable();
        var persons = data.Skip(offset);

        // Act
        var query = data.Query(QueryParams.Create(offset, QueryParams.DefaultLimit,
            QueryParams.DefaultSort, QueryParams.DefaultFilter).Value);
        var result = query.ToList();

        // Assert
        result.Should().ContainInOrder(persons);
    }

    [Fact]
    public void GivenQuery_WhenLimit_ThenReturnQuery()
    {
        // Arrange
        const int limit = 3;
        var data = Faker.Generate(5).AsQueryable();
        var persons = data.Take(limit);

        // Act
        var query = data.Query(QueryParams.Create(QueryParams.DefaultOffset, limit,
            QueryParams.DefaultSort, QueryParams.DefaultFilter).Value);
        var result = query.ToList();

        // Assert
        result.Should().ContainInOrder(persons);
    }

    private record MyId : StronglyTypedId<MyId>;

    private record FakeClass(MyId PersonId, string Name, decimal Bonus, bool IsWorking);
}
