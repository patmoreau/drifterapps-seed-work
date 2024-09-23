using Bogus;

namespace DrifterApps.Seeds.Domain.Tests;

[UnitTest]
public class ResultExtensionsTests
{
    private readonly Faker _faker = new();
    private readonly ResultError _testFirstError = new("TestFirstError", "Test Error");
    private readonly ResultError _testSecondError = new("TestSecondError", "Test Error");

    [Fact]
    public void GivenOnSuccess_WhenResultIsSuccess_ThenExecuteNext()
    {
        // Arrange
        var resultIn = Result.Success();

        // Act
        var result = resultIn.OnSuccess(Result.Success);

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public void GivenOnSuccess_WhenResultIsFailure_ThenReturnFailure()
    {
        // Arrange
        var resultIn = Result.Failure(_testFirstError);

        // Act
        var result = resultIn.OnSuccess(Result.Success);

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public void GivenOnSuccessOfTOut_WhenResultIsSuccess_ThenExecuteNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Result.Success();

        // Act
        var result = resultIn.OnSuccess(() => Result<string>.Success(expectedValue));

        // Assert
        result.Should().BeOfType<Result<string>>();
        result.Should().BeSuccessful().WithValue(expectedValue);
    }

    [Fact]
    public void GivenOnSuccessOfTOut_WhenResultIsFailure_ThenReturnFailure()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Result.Failure(_testFirstError);

        // Act
        var result = resultIn.OnSuccess(() => Result<string>.Success(expectedValue));

        // Assert
        result.Should().BeOfType<Result<string>>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public void GivenOnSuccessOfTIn_WhenResultIsSuccess_ThenExecuteNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Result<string>.Success(expectedValue);

        // Act
        var result = resultIn.OnSuccess(_ => Result.Success());

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public void GivenOnSuccessOfTIn_WhenResultIsFailure_ThenReturnFailure()
    {
        // Arrange
        var resultIn = Result<string>.Failure(_testFirstError);

        // Act
        var result = resultIn.OnSuccess(_ => Result.Success());

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public void GivenOnSuccessOfTInTOut_WhenResultIsSuccess_ThenExecuteNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Int();
        var resultIn = Result<string>.Success(_faker.Random.Word());

        // Act
        var result = resultIn.OnSuccess(value => Result<int>.Success(expectedValue));

        // Assert
        result.Should().BeOfType<Result<int>>();
        result.Should().BeSuccessful().WithValue(expectedValue);
    }

    [Fact]
    public void GivenOnSuccessOfTInTOut_WhenResultIsFailure_ThenReturnFailure()
    {
        // Arrange
        var expectedValue = _faker.Random.Int();
        var resultIn = Result<string>.Failure(_testFirstError);

        // Act
        var result = resultIn.OnSuccess(value => Result<int>.Success(expectedValue));

        // Assert
        result.Should().BeOfType<Result<int>>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public void GivenOnFailure_WhenResultIsSuccess_ThenReturnSuccess()
    {
        // Arrange
        var resultIn = Result.Success();

        // Act
        var result = resultIn.OnFailure(_ => Result.Failure(_testSecondError));

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public void GivenOnFailure_WhenResultIsFailure_ThenExecuteNext()
    {
        // Arrange
        var resultIn = Result.Failure(_testFirstError);
        var expectedError = ResultError.None;

        // Act
        var result = resultIn.OnFailure(error =>
        {
            expectedError = error;
            return Result.Failure(_testSecondError);
        });

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeFailure().WithError(_testSecondError);
        expectedError.Should().Be(_testFirstError);
    }

    [Fact]
    public void GivenOnFailureOfTIn_WhenResultIsSuccess_ThenReturnSuccess()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Result<string>.Success(expectedValue);

        // Act
        var result = resultIn.OnFailure(_ => Result<string>.Failure(_testSecondError));

        // Assert
        result.Should().BeOfType<Result<string>>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public void GivenOnFailureOfTIn_WhenResultIsFailure_ThenExecuteNext()
    {
        // Arrange
        var resultIn = Result<string>.Failure(_testFirstError);
        var expectedError = ResultError.None;

        // Act
        var result = resultIn.OnFailure(error =>
        {
            expectedError = error;
            return Result<string>.Failure(_testSecondError);
        });

        // Assert
        result.Should().BeOfType<Result<string>>();
        result.Should().BeFailure().WithError(_testSecondError);
        expectedError.Should().Be(_testFirstError);
    }

    [Fact]
    public void GivenMatch_WhenResultIsSuccess_ThenExecuteSuccessNext()
    {
        // Arrange
        var resultIn = Result.Success();

        // Act
        var result = resultIn.Match(Result.Success, Result.Failure);

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public void GivenMatch_WhenResultIsFailure_ThenExecuteFailureNext()
    {
        // Arrange
        var resultIn = Result.Failure(_testFirstError);

        // Act
        var result = resultIn.Match(Result.Success, Result.Failure);

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public void GivenMatchOfTIn_WhenResultIsSuccess_ThenExecuteSuccessNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Result<string>.Success(expectedValue);

        // Act
        var result = resultIn.Match(_ => Result.Success(), Result.Failure);

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public void GivenMatchOfTIn_WhenResultIsFailure_ThenExecuteFailureNext()
    {
        // Arrange
        var resultIn = Result<string>.Failure(_testFirstError);

        // Act
        var result = resultIn.Match(_ => Result.Success(), Result.Failure);

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public void GivenMatchOfTOut_WhenResultIsSuccess_ThenExecuteSuccessNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Result.Success();

        // Act
        var result = resultIn.Match(() => Result<string>.Success(expectedValue), Result<string>.Failure);

        // Assert
        result.Should().BeOfType<Result<string>>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public void GivenMatchOfTOut_WhenResultIsFailure_ThenExecuteFailureNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Result.Failure(_testFirstError);

        // Act
        var result = resultIn.Match(() => Result<string>.Success(expectedValue), Result<string>.Failure);

        // Assert
        result.Should().BeOfType<Result<string>>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public void GivenMatchOfTInTOut_WhenResultIsSuccess_ThenExecuteSuccessNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Result<string>.Success(expectedValue);

        // Act
        var result = resultIn.Match(_ => Result<int>.Success(_faker.Random.Int()), Result<int>.Failure);

        // Assert
        result.Should().BeOfType<Result<int>>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public void GivenMatchOfTInTOut_WhenResultIsFailure_ThenExecuteFailureNext()
    {
        // Arrange
        var resultIn = Result<string>.Failure(_testFirstError);

        // Act
        var result = resultIn.Match(_ => Result<int>.Success(_faker.Random.Int()), Result<int>.Failure);

        // Assert
        result.Should().BeOfType<Result<int>>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public async Task GivenAsyncOnSuccess_WhenResultIsSuccess_ThenExecuteNext()
    {
        // Arrange
        var resultIn = Task.FromResult(Result.Success());

        // Act
        var result = await resultIn.OnSuccess(() => Task.FromResult(Result.Success()));

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task GivenAsyncOnSuccess_WhenResultIsFailure_ThenReturnFailure()
    {
        // Arrange
        var resultIn = Task.FromResult(Result.Failure(_testFirstError));

        // Act
        var result = await resultIn.OnSuccess(() => Task.FromResult(Result.Success()));

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public async Task GivenAsyncOnSuccessOfTOut_WhenResultIsSuccess_ThenExecuteNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Task.FromResult(Result.Success());

        // Act
        var result = await resultIn.OnSuccess(() => Task.FromResult(Result<string>.Success(expectedValue)));

        // Assert
        result.Should().BeOfType<Result<string>>();
        result.Should().BeSuccessful().WithValue(expectedValue);
    }

    [Fact]
    public async Task GivenAsyncOnSuccessOfTOut_WhenResultIsFailure_ThenReturnFailure()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Task.FromResult(Result.Failure(_testFirstError));

        // Act
        var result = await resultIn.OnSuccess(() => Task.FromResult(Result<string>.Success(expectedValue)));

        // Assert
        result.Should().BeOfType<Result<string>>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public async Task GivenAsyncOnSuccessOfTIn_WhenResultIsSuccess_ThenExecuteNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Task.FromResult(Result<string>.Success(expectedValue));

        // Act
        var result = await resultIn.OnSuccess(_ => Task.FromResult(Result.Success()));

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task GivenAsyncOnSuccessOfTIn_WhenResultIsFailure_ThenReturnFailure()
    {
        // Arrange
        var resultIn = Task.FromResult(Result<string>.Failure(_testFirstError));

        // Act
        var result = await resultIn.OnSuccess(_ => Task.FromResult(Result.Success()));

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public async Task GivenAsyncOnSuccessOfTInTOut_WhenResultIsSuccess_ThenExecuteNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Int();
        var resultIn = Task.FromResult(Result<string>.Success(_faker.Random.Word()));

        // Act
        var result = await resultIn.OnSuccess(value => Task.FromResult(Result<int>.Success(expectedValue)));

        // Assert
        result.Should().BeOfType<Result<int>>();
        result.Should().BeSuccessful().WithValue(expectedValue);
    }

    [Fact]
    public async Task GivenAsyncOnSuccessOfTInTOut_WhenResultIsFailure_ThenReturnFailure()
    {
        // Arrange
        var expectedValue = _faker.Random.Int();
        var resultIn = Task.FromResult(Result<string>.Failure(_testFirstError));

        // Act
        var result = await resultIn.OnSuccess(value => Task.FromResult(Result<int>.Success(expectedValue)));

        // Assert
        result.Should().BeOfType<Result<int>>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public async Task GivenAsyncOnFailure_WhenResultIsSuccess_ThenReturnSuccess()
    {
        // Arrange
        var resultIn = Task.FromResult(Result.Success());

        // Act
        var result = await resultIn.OnFailure(_ => Task.FromResult(Result.Failure(_testSecondError)));

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task GivenAsyncOnFailure_WhenResultIsFailure_ThenExecuteNext()
    {
        // Arrange
        var resultIn = Task.FromResult(Result.Failure(_testFirstError));
        var expectedError = ResultError.None;

        // Act
        var result = await resultIn.OnFailure(error =>
        {
            expectedError = error;
            return Task.FromResult(Result.Failure(_testSecondError));
        });

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeFailure().WithError(_testSecondError);
        expectedError.Should().Be(_testFirstError);
    }

    [Fact]
    public async Task GivenAsyncOnFailureOfTIn_WhenResultIsSuccess_ThenReturnSuccess()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Task.FromResult(Result<string>.Success(expectedValue));

        // Act
        var result = await resultIn.OnFailure(_ => Task.FromResult<Result>(Result<string>.Failure(_testSecondError)));

        // Assert
        result.Should().BeOfType<Result<string>>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task GivenAsyncOnFailureOfTIn_WhenResultIsFailure_ThenExecuteNext()
    {
        // Arrange
        var resultIn = Task.FromResult(Result<string>.Failure(_testFirstError));
        var expectedError = ResultError.None;

        // Act
        var result = await resultIn.OnFailure(error =>
        {
            expectedError = error;
            return Task.FromResult<Result>(Result<string>.Failure(_testSecondError));
        });

        // Assert
        result.Should().BeOfType<Result<string>>();
        result.Should().BeFailure().WithError(_testSecondError);
        expectedError.Should().Be(_testFirstError);
    }

    [Fact]
    public async Task GivenAsyncMatch_WhenResultIsSuccess_ThenExecuteSuccessNext()
    {
        // Arrange
        var resultIn = Task.FromResult(Result.Success());

        // Act
        var result = await resultIn.Match(() => Task.FromResult(Result.Success()),
            error => Task.FromResult(Result.Failure(error)));

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task GivenAsyncMatch_WhenResultIsFailure_ThenExecuteFailureNext()
    {
        // Arrange
        var resultIn = Task.FromResult(Result.Failure(_testFirstError));

        // Act
        var result = await resultIn.Match(() => Task.FromResult(Result.Success()),
            error => Task.FromResult(Result.Failure(error)));

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public async Task GivenAsyncMatchOfTIn_WhenResultIsSuccess_ThenExecuteSuccessNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Task.FromResult(Result<string>.Success(expectedValue));

        // Act
        var result = await resultIn.Match(_ => Task.FromResult(Result.Success()),
            error => Task.FromResult(Result.Failure(error)));

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task GivenAsyncMatchOfTIn_WhenResultIsFailure_ThenExecuteFailureNext()
    {
        // Arrange
        var resultIn = Task.FromResult(Result<string>.Failure(_testFirstError));

        // Act
        var result = await resultIn.Match(_ => Task.FromResult(Result.Success()),
            error => Task.FromResult(Result.Failure(error)));

        // Assert
        result.Should().BeOfType<Result>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public async Task GivenAsyncMatchOfTOut_WhenResultIsSuccess_ThenExecuteSuccessNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Task.FromResult(Result.Success());

        // Act
        var result = await resultIn.Match(() => Task.FromResult(Result<string>.Success(expectedValue)),
            error => Task.FromResult(Result<string>.Failure(error)));

        // Assert
        result.Should().BeOfType<Result<string>>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task GivenAsyncMatchOfTOut_WhenResultIsFailure_ThenExecuteFailureNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Task.FromResult(Result.Failure(_testFirstError));

        // Act
        var result = await resultIn.Match(() => Task.FromResult(Result<string>.Success(expectedValue)),
            error => Task.FromResult(Result<string>.Failure(error)));

        // Assert
        result.Should().BeOfType<Result<string>>();
        result.Should().BeFailure().WithError(_testFirstError);
    }

    [Fact]
    public async Task GivenAsyncMatchOfTInTOut_WhenResultIsSuccess_ThenExecuteSuccessNext()
    {
        // Arrange
        var expectedValue = _faker.Random.Word();
        var resultIn = Task.FromResult(Result<string>.Success(expectedValue));

        // Act
        var result = await resultIn.Match(_ => Task.FromResult(Result<int>.Success(_faker.Random.Int())),
            error => Task.FromResult(Result<int>.Failure(error)));

        // Assert
        result.Should().BeOfType<Result<int>>();
        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task GivenAsyncMatchOfTInTOut_WhenResultIsFailure_ThenExecuteFailureNext()
    {
        // Arrange
        var resultIn = Task.FromResult(Result<string>.Failure(_testFirstError));

        // Act
        var result = await resultIn.Match(_ => Task.FromResult(Result<int>.Success(_faker.Random.Int())),
            error => Task.FromResult(Result<int>.Failure(error)));

        // Assert
        result.Should().BeOfType<Result<int>>();
        result.Should().BeFailure().WithError(_testFirstError);
    }
}
