using DrifterApps.Seeds.Application.Mediatr.Tests.Mocks;
using DrifterApps.Seeds.Testing;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Categories;

namespace DrifterApps.Seeds.Application.Mediatr.Tests;

[UnitTest]
public class LoggingBehaviorTests
{
    private readonly LoggingBehaviorDriver _driver = new();

    [Fact]
    public async Task Handle_WithRequest_LogsStartAndEnd()
    {
        // Arrange
        var sut = _driver.Build();

        // Act
        await sut.Handle(_driver.Request, _driver.Next, CancellationToken.None);

        // Assert
        _driver.ShouldHaveLoggedRequestStarted()
            .ShouldHaveLoggedRequestEnded();
    }

    private class LoggingBehaviorDriver : IDriverOf<LoggingBehavior<SampleRequest, SampleResponse>>
    {
        private readonly LoggerMock<LoggingBehavior<SampleRequest, SampleResponse>> _logger =
            Substitute.ForPartsOf<LoggerMock<LoggingBehavior<SampleRequest, SampleResponse>>>();

        public SampleRequest Request { get; } = new() {Name = "John Doe", Age = 30};

        public RequestHandlerDelegate<SampleResponse> Next { get; } =
            Substitute.For<RequestHandlerDelegate<SampleResponse>>();

        public LoggingBehavior<SampleRequest, SampleResponse> Build() => new(_logger);

        public LoggingBehaviorDriver ShouldHaveLoggedRequestStarted()
        {
            _logger.Received(1).Log(
                Arg.Is(LogLevel.Information),
                Arg.Any<EventId>(),
                Arg.Is<string>(message =>
                    message.StartsWith($"API Request started: {typeof(SampleRequest).FullName}")));

            return this;
        }

        public LoggingBehaviorDriver ShouldHaveLoggedRequestEnded()
        {
            _logger.Received(1).Log(
                Arg.Is(LogLevel.Information),
                Arg.Any<EventId>(),
                Arg.Is<string>(message =>
                    message.StartsWith($"API Request ended: {typeof(SampleRequest).FullName}")));

            return this;
        }
    }

    internal sealed class SampleRequest : IRequest<SampleResponse>
    {
        public string Name { get; set; } = null!;
        public int Age { get; set; }
    }

    internal sealed class SampleResponse
    {
    }
}
