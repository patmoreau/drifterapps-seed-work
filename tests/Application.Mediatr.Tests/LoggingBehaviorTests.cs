using DrifterApps.Seeds.Application.Mediatr.Tests.Mocks;
using DrifterApps.Seeds.Testing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Seeds.Application.Mediatr.Tests;

[UnitTest]
public class LoggingBehaviorTests
{
    private readonly Driver _driver = new();

    [Fact]
    public async Task Handle_WithRequest_LogsStartAndEnd()
    {
        // Arrange
        var sut = _driver.Build();

        // Act
        await sut.Handle(_driver.Request, _driver.Next, CancellationToken.None);

        // Assert
        _driver
            .ShouldHaveLoggedRequestStarted()
            .ShouldHaveLoggedRequestEnded();
    }

    private class Driver : IDriverOf<LoggingBehavior<IRequest<string>, string>>
    {
        private readonly LoggerMock<IRequest<string>> _logger = Substitute.ForPartsOf<LoggerMock<IRequest<string>>>();

        public IRequest<string> Request { get; } = Substitute.For<IRequest<string>>();

        public RequestHandlerDelegate<string> Next { get; } = Substitute.For<RequestHandlerDelegate<string>>();

        public LoggingBehavior<IRequest<string>, string> Build() => new(_logger);

        public Driver ShouldHaveLoggedRequestStarted()
        {
            _logger.Received(1).Log(
                Arg.Is(LogLevel.Information),
                Arg.Any<EventId>(),
                Arg.Is<string>(message =>
                    message.StartsWith($"API Request started: {typeof(IRequest<string>).FullName}")));

            return this;
        }

        public Driver ShouldHaveLoggedRequestEnded()
        {
            _logger.Received(1).Log(
                Arg.Is(LogLevel.Information),
                Arg.Any<EventId>(),
                Arg.Is<string>(message =>
                    message.StartsWith($"API Request ended: {typeof(IRequest<string>).FullName}")));

            return this;
        }
    }
}
