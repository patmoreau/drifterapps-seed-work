using FluentValidation;
using Xunit.Categories;

namespace DrifterApps.Seeds.Application.Mediatr.Tests;

[UnitTest]
public class ValidationPreProcessorTests
{
    [Fact]
    public async Task Process_WithNoValidators_ShouldNotThrowException()
    {
        // Arrange
        var preProcessor = new ValidationPreProcessor<SampleRequest>(Array.Empty<IValidator<SampleRequest>>());

        // Act
        var act = async () => await preProcessor.Process(new SampleRequest(), CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Process_WithValidRequest_ShouldNotThrowException()
    {
        // Arrange
        var validator = new SampleRequestValidator();
        var preProcessor =
            new ValidationPreProcessor<SampleRequest>(new List<IValidator<SampleRequest>> {validator});

        // Act
        var act = async () =>
            await preProcessor.Process(new SampleRequest {Name = "John Doe", Age = 30}, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Process_WithInvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var validator = new SampleRequestValidator();
        var preProcessor =
            new ValidationPreProcessor<SampleRequest>(new List<IValidator<SampleRequest>> {validator});

        // Act
        var act = async () =>
            await preProcessor.Process(new SampleRequest {Name = "", Age = -5}, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(
                "Validation failed: \r\n -- Name: 'Name' must not be empty. Severity: Error\r\n -- Age: 'Age' must be greater than or equal to '0'. Severity: Error");
    }
}

public class SampleRequest
{
    public string Name { get; set; } = null!;
    public int Age { get; set; }
}

public class SampleRequestValidator : AbstractValidator<SampleRequest>
{
    public SampleRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Age).GreaterThanOrEqualTo(0);
    }
}
