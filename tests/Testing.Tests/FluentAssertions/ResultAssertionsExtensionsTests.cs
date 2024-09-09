using Bogus;
using DrifterApps.Seeds.Domain;

namespace DrifterApps.Seeds.Testing.Tests.FluentAssertions;

[UnitTest]
public class ResultAssertionsExtensionsTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void GivenBeSuccessfulResult_WhenIsSuccess_ThenDontAssert()
    {
        // arrange
        var result = Result.Success();

        // act
        Action act = () => result.Should().BeSuccessful();

        // assert
        act.Should().NotThrow<XunitException>();
    }

    [Fact]
    public void GivenBeSuccessfulResult_WhenIsFailure_ThenAssert()
    {
        // arrange
        var result = Result.Failure(new ResultError(_faker.Random.Word(), _faker.Random.Words()));

        // act
        Action act = () => result.Should().BeSuccessful();

        // assert
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void GivenBeFailureResult_WhenIsFailure_ThenDontAssert()
    {
        // arrange
        var result = Result.Failure(new ResultError(_faker.Random.Word(), _faker.Random.Words()));

        // act
        Action act = () => result.Should().BeFailure();

        // assert
        act.Should().NotThrow<XunitException>();
    }

    [Fact]
    public void GivenBeFailureResult_WhenIsSuccessful_ThenAssert()
    {
        // arrange
        var result = Result.Success();

        // act
        Action act = () => result.Should().BeFailure();

        // assert
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void GivenBeSuccessfulResultOfT_WhenIsSuccess_ThenDontAssert()
    {
        // arrange
        var value = _faker.Random.Word();
        var result = Result<string>.Success(value);

        // act
        Action act = () => result.Should().BeSuccessful();

        // assert
        act.Should().NotThrow<XunitException>();
    }

    [Fact]
    public void GivenBeSuccessfulResultOfT_WhenIsFailure_ThenAssert()
    {
        // arrange
        var result = Result<string>.Failure(new ResultError(_faker.Random.Word(), _faker.Random.Words()));

        // act
        Action act = () => result.Should().BeSuccessful();

        // assert
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void GivenBeFailureResultOfT_WhenIsFailure_ThenDontAssert()
    {
        // arrange
        var result = Result<string>.Failure(new ResultError(_faker.Random.Word(), _faker.Random.Words()));

        // act
        Action act = () => result.Should().BeFailure();

        // assert
        act.Should().NotThrow<XunitException>();
    }

    [Fact]
    public void GivenBeFailureResultOfT_WhenIsSuccessful_ThenAssert()
    {
        // arrange
        var value = _faker.Random.Word();
        var result = Result<string>.Success(value);

        // act
        Action act = () => result.Should().BeFailure();

        // assert
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void GivenHaveValueResultOfT_WhenIsSuccess_ThenDontAssert()
    {
        // arrange
        var value = _faker.Random.Word();
        var result = Result<string>.Success(value);

        // act
        Action act = () => result.Should().HaveValue(value);

        // assert
        act.Should().NotThrow<XunitException>();
    }

    [Fact]
    public void GivenHaveValueResultOfT_WhenIsFailure_ThenAssert()
    {
        // arrange
        var value = _faker.Random.Word();
        var result = Result<string>.Failure(new ResultError(_faker.Random.Word(), _faker.Random.Words()));

        // act
        Action act = () => result.Should().HaveValue(value);

        // assert
        act.Should().Throw<XunitException>();
    }
}
