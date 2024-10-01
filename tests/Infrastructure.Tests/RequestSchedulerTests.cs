using DrifterApps.Seeds.Infrastructure;
using DrifterApps.Seeds.Testing;
using DrifterApps.Seeds.Testing.Attributes;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Infrastructure.Tests;

[UnitTest]
public class RequestSchedulerTests
{
    private readonly Driver _driver = new();

    [Fact]
    public void GivenConstructor_WhenRequestExecutorIsNull_ThenThrowArgumentNullException()
    {
        // arrange
        _driver.WhenRequestExecutorIsNull();

        // act
        Action action = () => _driver.Build();

        // assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GivenSendNow_WhenInvoked_ThenQueueTheJob()
    {
        // arrange
        var sut = _driver.Build();

        // act
        sut.SendNow(new MockRequest(), Fakerizer.Lorem.Sentence());

        // assert
        _driver.ShouldHaveJobEnqueued();
    }

    [Fact]
    public void GivenSchedule_WhenDateTimeOffsetUsedInvoked_ThenScheduleTheJob()
    {
        // arrange
        var sut = _driver.Build();

        // act
        sut.Schedule(new MockRequest(), Fakerizer.Date.FutureOffset(), Fakerizer.Lorem.Sentence());

        // assert
        _driver.ShouldHaveJobScheduled();
    }

    [Fact]
    public void GivenSchedule_WhenTimeSpanUsedInvoked_ThenScheduleTheJob()
    {
        // arrange
        var sut = _driver.Build();

        // act
        sut.Schedule(new MockRequest(), Fakerizer.Date.Timespan(), Fakerizer.Lorem.Sentence());

        // assert
        _driver.ShouldHaveJobScheduled();
    }

    [Fact]
    public void GivenScheduleRecurring_WhenInvoked_ThenScheduleTheRecurringJob()
    {
        // arrange
        var sut = _driver.Build();

        // act
        sut.ScheduleRecurring(new MockRequest(), Fakerizer.Lorem.Word(), Fakerizer.Random.Word(),
            Fakerizer.Lorem.Sentence());

        // assert
        _driver.ShouldHaveRecurringJobScheduled();
    }

    private sealed class Driver : IDriverOf<RequestScheduler>
    {
        private readonly IBackgroundJobClientV2 _backgroundJobClient;
        private readonly IBackgroundJobClientFactoryV2 _backgroundJobFactory;
        private readonly IJsonSerializerOptionsFactory _jsonSerializerOptionsFactory;
        private readonly ILogger<RequestScheduler> _logger;
        private readonly IRecurringJobManagerFactoryV2 _recurringJobFactory;
        private readonly IRecurringJobManagerV2 _recurringJobManager;
        private IRequestExecutor _requestExecutor;

        public Driver()
        {
            _requestExecutor = Substitute.For<IRequestExecutor>();
            _backgroundJobFactory = Substitute.For<IBackgroundJobClientFactoryV2>();
            _backgroundJobClient = Substitute.For<IBackgroundJobClientV2>();
            _backgroundJobFactory.GetClientV2(Arg.Any<JobStorage>()).Returns(_backgroundJobClient);
            _recurringJobFactory = Substitute.For<IRecurringJobManagerFactoryV2>();
            _recurringJobManager = Substitute.For<IRecurringJobManagerV2>();
            _recurringJobFactory.GetManagerV2(Arg.Any<JobStorage>()).Returns(_recurringJobManager);
            JobStorage.Current = Substitute.ForPartsOf<JobStorage>();
            _logger = Substitute.For<ILogger<RequestScheduler>>();
            _jsonSerializerOptionsFactory = Substitute.For<IJsonSerializerOptionsFactory>();
        }

        public RequestScheduler Build() => new(_requestExecutor, _backgroundJobFactory, _recurringJobFactory,
            _jsonSerializerOptionsFactory, _logger);

        public Driver WhenRequestExecutorIsNull()
        {
            _requestExecutor = null!;

            return this;
        }

        [AssertionMethod]
        public Driver ShouldHaveJobEnqueued()
        {
            _backgroundJobClient.Received(1).Create(Arg.Any<Job>(), Arg.Any<EnqueuedState>());

            return this;
        }

        [AssertionMethod]
        public Driver ShouldHaveJobScheduled()
        {
            _backgroundJobClient.Received(1).Create(Arg.Any<Job>(), Arg.Any<ScheduledState>());

            return this;
        }

        [AssertionMethod]
        public Driver ShouldHaveRecurringJobScheduled()
        {
            _recurringJobManager.Received(1).AddOrUpdate(Arg.Any<string>(), Arg.Any<Job>(), Arg.Any<string>(),
                Arg.Any<RecurringJobOptions>());

            return this;
        }
    }

    private record MockRequest : IRequest;
}
