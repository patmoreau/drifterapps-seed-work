using System;

namespace DrifterApps.Seeds.Domain.Tests;

[UnitTest]
public class DomainExceptionOfTTests
{
    [Fact]
    public void Constructor_WithoutArguments_ShouldCreateInstance()
    {
        // Act
        var exception = new DomainException<ContextClass>();

        // Assert
        using var scope = new AssertionScope();
        exception.Should().NotBeNull();
        exception.Message.Should()
            .Be(
                "Exception of type 'DrifterApps.Seeds.Domain.DomainException`1[DrifterApps.Seeds.Domain.Tests.DomainExceptionOfTTests+ContextClass]' was thrown.");
        exception.InnerException.Should().BeNull();
        exception.Context.Should().Be("ContextClass");
    }

    [Fact]
    public void Constructor_WithMessage_ShouldCreateInstanceWithMessage()
    {
        // Arrange
        var errorMessage = "Test error message";

        // Act
        var exception = new DomainException<ContextClass>(errorMessage);

        // Assert
        using var scope = new AssertionScope();
        exception.Should().NotBeNull();
        exception.Message.Should().Be(errorMessage);
        exception.InnerException.Should().BeNull();
        exception.Context.Should().Be("ContextClass");
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldCreateInstanceWithMessageAndInnerException()
    {
        // Arrange
        const string errorMessage = "Test error message";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new DomainException<ContextClass>(errorMessage, innerException);

        // Assert
        using var scope = new AssertionScope();
        exception.Should().NotBeNull();
        exception.Message.Should().Be(errorMessage);
        exception.InnerException.Should().Be(innerException);
        exception.Context.Should().Be("ContextClass");
    }

    private record ContextClass;
}
