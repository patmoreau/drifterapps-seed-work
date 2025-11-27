using DrifterApps.Seeds.Infrastructure;
using DrifterApps.Seeds.Testing;
using DrifterApps.Seeds.Testing.Attributes;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Infrastructure.Tests;

[UnitTest]
public class RequestSchedulerTests
{
    private readonly Driver _driver = new();

    [Fact]
    public void GivenSendNow_WhenInvoked_ThenQueueTheJob()
    {
        // arrange
        var sut = _driver.Build();

        // act
        sut.QueueHandler<MockHandler>(handler => handler.Handle(CancellationToken.None), Fakerizer.Lorem.Sentence());

        // assert
        _driver.ShouldHaveJobEnqueued();
    }

    private sealed class Driver : IDriverOf<RequestScheduler>
    {
        private readonly IBackgroundJobClientV2 _backgroundJobClient;
        private readonly IBackgroundJobClientFactoryV2 _backgroundJobFactory;
        private readonly ILogger<RequestScheduler> _logger;

        public Driver()
        {
            _backgroundJobFactory = Substitute.For<IBackgroundJobClientFactoryV2>();
            _backgroundJobClient = Substitute.For<IBackgroundJobClientV2>();
            _backgroundJobFactory.GetClientV2(Arg.Any<JobStorage>()).Returns(_backgroundJobClient);
            JobStorage.Current = Substitute.ForPartsOf<JobStorage>();
            _logger = Substitute.For<ILogger<RequestScheduler>>();
        }

        public RequestScheduler Build() => new(_backgroundJobFactory, _logger);

        [AssertionMethod]
        public Driver ShouldHaveJobEnqueued()
        {
            _backgroundJobClient.Received(1).Create(Arg.Any<Job>(), Arg.Any<EnqueuedState>());

            return this;
        }
    }

    private class MockHandler
    {
#pragma warning disable CA1822
        public Task Handle(CancellationToken cancellationToken) => Task.CompletedTask;
#pragma warning restore CA1822
    }
}
