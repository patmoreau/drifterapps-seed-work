using Bogus;

namespace DrifterApps.Seeds.Domain.Tests;

[UnitTest]
public class ResultTests
{
    private readonly Faker _faker = new();

    private readonly ResultError _idError = new("errorId", "Error Id");
    private readonly ResultError _otherError = new("errorOther", "Error Other");

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

    [Fact]
    public void GivenValidate_WhenValidationFuncReturnTrue_ThenReturnSuccess()
    {
        // Arrange

        // Act
        var result = Result.Validate(MyIdValidation(Guid.NewGuid()));

        // Assert
        result.Should().BeSuccessful();
    }

    [Fact]
    public void GivenValidate_WhenValidationFuncReturnFalse_ThenReturnFailureWithError()
    {
        // Arrange

        // Act
        var result = Result.Validate(MyIdValidation(Guid.Empty));

        // Assert
        result.Should().BeFailure()
            .WithError(new ResultAggregateError(Result.CodeValidateErrors, "1 validation failed", [_idError]));
    }

    [Fact]
    public void GivenValidate_WhenAllValidationFuncReturnTrue_ThenReturnSuccess()
    {
        // Arrange

        // Act
        var result = Result.Validate(MyIdValidation(Guid.NewGuid()), MyOtherValidation(1));

        // Assert
        result.Should().BeSuccessful();
    }

    [Fact]
    public void GivenValidate_WhenMixValidationFuncReturnFalse_ThenReturnFailureWithErrors()
    {
        // Arrange

        // Act
        var result = Result.Validate(MyIdValidation(Guid.NewGuid()), MyOtherValidation(0));

        // Assert
        result.Should().BeFailure()
            .WithError(new ResultAggregateError(Result.CodeValidateErrors, "1 validation failed", [_otherError]));
    }

    [Fact]
    public void GivenValidate_WhenAllValidationFuncReturnFalse_ThenReturnFailureWithErrors()
    {
        // Arrange

        // Act
        var result = Result.Validate(MyIdValidation(Guid.Empty), MyOtherValidation(0));

        // Assert
        result.Should().BeFailure()
            .WithError(new ResultAggregateError(Result.CodeValidateErrors, "2 validations failed",
                [_idError, _otherError]));
    }

    private ResultValidation MyIdValidation(Guid id) => ResultValidation.Create(() => id != Guid.Empty, _idError);

    private ResultValidation MyOtherValidation(int count) => ResultValidation.Create(() => count > 0, _otherError);

    private ResultError CreateError() => new(_faker.Random.Hash(), _faker.Lorem.Sentence());

    private record MyResult(bool IsSuccess, ResultError Error) : Result(IsSuccess, Error);
}
