using Bogus;

namespace DrifterApps.Seeds.Domain.Tests;

[UnitTest]
public class ResultTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void GivenConstructor_WhenSuccessAndErrorIsNone_ThenIsSuccessIsTrue()
    {
        // Arrange

        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(ResultError.None);
    }

    [Fact]
    public void GivenConstructor_WhenSuccessAndError_ThenThrowArgumentException()
    {
        // Arrange

        // Act
        Action act = () => _ = new MyResult(true, CreateError());

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Invalid error (Parameter 'error')");
    }

    [Fact]
    public void GivenConstructor_WhenFailureAndErrorIsNotNone_ThenIsFailureIsTrue()
    {
        // Arrange
        var error = CreateError();

        // Act
        var result = Result.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void GivenConstructor_WhenFailureAndErrorIsNone_ThenThrowArgumentException()
    {
        // Arrange

        // Act
        Action act = () => _ = new MyResult(false, ResultError.None);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Invalid error (Parameter 'error')");
    }

    [Fact]
    public void GivenConstructorOfT_WhenSuccessAndValue_ThenIsSuccessIsTrue()
    {
        // Arrange
        var value = _faker.Random.Hash();

        // Act
        var result = Result<string>.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(ResultError.None);
        result.Value.Should().NotBeNull().And.Be(value);
    }

    [Fact]
    public void GivenConstructor_WhenSuccessAndNoValue_ThenThrowArgumentException()
    {
        // Arrange

        // Act
        Action act = () => _ = Result<string>.Success(null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Value cannot be null when the operation is successful. (Parameter 'value')");
    }

    [Fact]
    public void GivenConstructorOfT_WhenFailureAndErrorIsNotNone_ThenIsFailureIsTrue()
    {
        // Arrange
        var error = CreateError();

        // Act
        var result = Result<string>.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        result.Value.Should().BeNull();
    }

    [Fact]
    public void GivenValue_WhenFailure_ThenThrowException()
    {
        // Arrange
        var error = CreateError();
        var result = Result<string>.Failure(error);

        // Act
        Action act = () => _ = result.Value;

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("Cannot access the value of a failed result.");
    }

    private ResultError CreateError() => new(_faker.Random.Hash(), _faker.Lorem.Sentence());

    private class MyResult(bool isSuccess, ResultError error) : Result(isSuccess, error);
}
