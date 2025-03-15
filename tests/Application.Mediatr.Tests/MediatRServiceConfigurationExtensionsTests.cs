using DrifterApps.Seeds.Domain;
using DrifterApps.Seeds.FluentResult;
using DrifterApps.Seeds.Testing;
using DrifterApps.Seeds.Testing.Attributes;
using FluentAssertions.Execution;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Seeds.Application.Mediatr.Tests;

[UnitTest]
public class MediatRServiceConfigurationExtensionsTests : IAsyncDisposable
{
    private readonly Driver _driver = new();

    public async ValueTask DisposeAsync()
    {
        await _driver.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void GivenRegisterServicesFromApplication_WhenInvoked_ThenRegisterServices()
    {
        // arrange
        _driver.GivenMediatrIsConfigured();

        // act
        _driver.Build();

        // assert
        _driver.ShouldContainAllExpectedBehaviors();
    }

    [Fact]
    public void GivenPingWithPongResponse_WhenNoFirstMessage_ThenValidatorIsCalledAndThrowsValidationException()
    {
        // arrange
        var mediator = _driver.GivenMediatrIsConfigured()
            .GivenPingPongGameIsReady()
            .Build();

        // act
        var action = async () => await mediator.Send(new TestRequests.PingWithPongResponse(string.Empty, false));

        // assert
        using var scope = new AssertionScope();
        action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public void GivenPingWithIntResponse_WhenNoFirstMessage_ThenValidatorIsCalledAndThrowsValidationException()
    {
        // arrange
        var mediator = _driver.GivenMediatrIsConfigured()
            .GivenPingPongGameIsReady()
            .Build();

        // act
        var action = async () => await mediator.Send(new TestRequests.PingWithIntResponse(string.Empty, false));

        // assert
        using var scope = new AssertionScope();
        action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public void GivenPingWithNoResponse_WhenNoFirstMessage_ThenValidatorIsCalledAndThrowsValidationException()
    {
        // arrange
        var mediator = _driver.GivenMediatrIsConfigured()
            .GivenPingPongGameIsReady()
            .Build();

        // act
        var action = async () => await mediator.Send(new TestRequests.PingWithNoResponse(string.Empty, false));

        // assert
        using var scope = new AssertionScope();
        action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GivenPingWithPongResult_WhenNoFirstMessage_ThenValidatorIsCalledAndReturnsResultFailure()
    {
        // arrange
        var mediator = _driver.GivenMediatrIsConfigured()
            .GivenPingPongGameIsReady()
            .Build();

        // act
        var result = await mediator.Send(new TestRequests.PingWithPongResult(string.Empty, false));

        // assert
        using var scope = new AssertionScope();
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ResultErrorAggregate>();
        result.Error.As<ResultErrorAggregate>().Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task GivenPingWithResult_WhenNoFirstMessage_ThenValidatorIsCalledAndReturnsResultFailure()
    {
        // arrange
        var mediator = _driver.GivenMediatrIsConfigured()
            .GivenPingPongGameIsReady()
            .Build();

        // act
        var result = await mediator.Send(new TestRequests.PingWithResult(string.Empty, false));

        // assert
        using var scope = new AssertionScope();
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ResultErrorAggregate>();
        result.Error.As<ResultErrorAggregate>().Errors.Should().HaveCount(1);
    }

    [Fact]
    public void GivenPing_WhenMissed_ThenRollback()
    {
        // arrange
        var mediator = _driver.GivenMediatrIsConfigured()
            .GivenPingPongGameIsReady()
            .Build();

        // act
        var action = async () =>
            await mediator.Send(new TestRequests.PingWithPongResponse(nameof(TestRequests.PingWithPongResponse), true));

        // assert
        using var scope = new AssertionScope();
        action.Should().ThrowAsync<TestExceptions.PingException>();
        _driver
            .ShouldHaveCalledUnitOfWorkBeginWorkAsync()
            .ShouldHaveCalledUnitOfWorkRollbackWorkAsync()
            .ShouldNotHaveCalledUnitOfWorkCommitWorkAsync();
    }

    [Fact]
    public async Task GivenPing_WhenHitBack_ThenCommit()
    {
        // arrange
        var mediator = _driver.GivenMediatrIsConfigured()
            .GivenPingPongGameIsReady()
            .Build();

        // act
        var pong = await mediator.Send(
            new TestRequests.PingWithPongResponse(nameof(TestRequests.PingWithPongResponse), false));

        // assert
        using var scope = new AssertionScope();
        pong.Should().NotBeNull().And.BeEquivalentTo(new TestResponses.Pong("PingWithPongResponse Pong"));
        _driver
            .ShouldHaveCalledUnitOfWorkBeginWorkAsync()
            .ShouldHaveCalledUnitOfWorkCommitWorkAsync()
            .ShouldNotHaveCalledUnitOfWorkRollbackWorkAsync();
    }

    private class Driver : IDriverOf<IMediator>, IAsyncDisposable
    {
        private readonly ILogger<TestRequests.PingWithIntResponse> _loggerWithIntResponse;
        private readonly ILogger<TestRequests.PingWithNoResponse> _loggerWithNoResponse;
        private readonly ILogger<TestRequests.PingWithPongResponse> _loggerWithPongResponse;
        private readonly ILogger<TestRequests.PingWithPongResult> _loggerWithPongResult;
        private readonly ILogger<TestRequests.PingWithResult> _loggerWithResult;
        private readonly ServiceCollection _serviceCollection;
        private readonly IUnitOfWork _unitOfWork;
        private ServiceProvider? _serviceProvider;

        public Driver()
        {
            _serviceCollection = [];
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _loggerWithPongResponse = Substitute.For<ILogger<TestRequests.PingWithPongResponse>>();
            _loggerWithIntResponse = Substitute.For<ILogger<TestRequests.PingWithIntResponse>>();
            _loggerWithNoResponse = Substitute.For<ILogger<TestRequests.PingWithNoResponse>>();
            _loggerWithPongResult = Substitute.For<ILogger<TestRequests.PingWithPongResult>>();
            _loggerWithResult = Substitute.For<ILogger<TestRequests.PingWithResult>>();
        }

        public async ValueTask DisposeAsync()
        {
            await _serviceProvider!.DisposeAsync();
            await _unitOfWork.DisposeAsync();
        }

        public IMediator Build()
        {
            _serviceProvider = _serviceCollection.BuildServiceProvider();
            return _serviceProvider.GetRequiredService<IMediator>();
        }

        public Driver GivenMediatrIsConfigured()
        {
            _serviceCollection.AddMediatR(config => config
                .RegisterServicesFromApplicationSeeds()
                .RegisterServicesFromAssembly(typeof(MediatRServiceConfigurationExtensionsTests).Assembly));

            return this;
        }

        public Driver GivenPingPongGameIsReady()
        {
            _serviceCollection.AddTransient<IUnitOfWork>(_ => _unitOfWork);
            _serviceCollection.AddTransient<ILogger<TestRequests.PingWithPongResponse>>(_ => _loggerWithPongResponse);
            _serviceCollection.AddTransient<ILogger<TestRequests.PingWithIntResponse>>(_ => _loggerWithIntResponse);
            _serviceCollection.AddTransient<ILogger<TestRequests.PingWithNoResponse>>(_ => _loggerWithNoResponse);
            _serviceCollection.AddTransient<ILogger<TestRequests.PingWithPongResult>>(_ => _loggerWithPongResult);
            _serviceCollection.AddTransient<ILogger<TestRequests.PingWithResult>>(_ => _loggerWithResult);
            _serviceCollection
                .AddTransient<IValidator<TestRequests.PingWithPongResponse>,
                    TestValidators.PingWithPongResponseValidator>();
            _serviceCollection
                .AddTransient<IValidator<TestRequests.PingWithIntResponse>,
                    TestValidators.PingWithIntResponseValidator>();
            _serviceCollection
                .AddTransient<IValidator<TestRequests.PingWithNoResponse>,
                    TestValidators.PingWithNoResponseValidator>();
            _serviceCollection
                .AddTransient<IValidator<TestRequests.PingWithPongResult>,
                    TestValidators.PingWithPongResultValidator>();
            _serviceCollection
                .AddTransient<IValidator<TestRequests.PingWithResult>,
                    TestValidators.PingWithResultValidator>();

            return this;
        }

        [AssertionMethod]
        public void ShouldContainAllExpectedBehaviors()
        {
            using var scope = new AssertionScope();

            _serviceCollection.Should().Contain(serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(IPipelineBehavior<,>) &&
                serviceDescriptor.ImplementationType == typeof(LoggingBehavior<,>) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient);
            _serviceCollection.Should().Contain(serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(IPipelineBehavior<,>) &&
                serviceDescriptor.ImplementationType == typeof(UnitOfWorkBehavior<,>) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient);
            _serviceCollection.Should().Contain(serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(IPipelineBehavior<,>) &&
                serviceDescriptor.ImplementationType == typeof(ValidationBehavior<,>) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient);
        }

        [AssertionMethod]
        public Driver ShouldNotHaveCalledUnitOfWorkBeginWorkAsync()
        {
            _unitOfWork.Received(0).BeginWorkAsync(Arg.Any<CancellationToken>());

            return this;
        }

        [AssertionMethod]
        public Driver ShouldHaveCalledUnitOfWorkBeginWorkAsync()
        {
            _unitOfWork.Received(1).BeginWorkAsync(Arg.Any<CancellationToken>());

            return this;
        }

        [AssertionMethod]
        public Driver ShouldNotHaveCalledUnitOfWorkRollbackWorkAsync()
        {
            _unitOfWork.Received(0).RollbackWorkAsync(Arg.Any<CancellationToken>());

            return this;
        }

        [AssertionMethod]
        public Driver ShouldHaveCalledUnitOfWorkRollbackWorkAsync()
        {
            _unitOfWork.Received(1).RollbackWorkAsync(Arg.Any<CancellationToken>());

            return this;
        }

        [AssertionMethod]
        public Driver ShouldNotHaveCalledUnitOfWorkCommitWorkAsync()
        {
            _unitOfWork.Received(0).CommitWorkAsync(Arg.Any<CancellationToken>());

            return this;
        }

        [AssertionMethod]
        public Driver ShouldHaveCalledUnitOfWorkCommitWorkAsync()
        {
            _unitOfWork.Received(1).CommitWorkAsync(Arg.Any<CancellationToken>());

            return this;
        }
    }

    internal static class TestRequests
    {
        internal record PingWithPongResponse(string Message, bool Missed)
            : IRequest<TestResponses.Pong>, IUnitOfWorkRequest;

        internal record PingWithIntResponse(string Message, bool Missed) : IRequest<int>, IUnitOfWorkRequest;

        internal record PingWithNoResponse(string Message, bool Missed) : IRequest, IUnitOfWorkRequest;

        internal record PingWithResult(string Message, bool Missed) : IRequest<Result<Nothing>>, IUnitOfWorkRequest;

        internal record PingWithPongResult(string Message, bool Missed)
            : IRequest<Result<TestResponses.Pong>>, IUnitOfWorkRequest;
    }

    internal static class TestExceptions
    {
        internal class PingException : Exception;
    }

    internal static class TestValidators
    {
        internal class PingWithPongResponseValidator : AbstractValidator<TestRequests.PingWithPongResponse>
        {
            public PingWithPongResponseValidator() => RuleFor(x => x.Message).NotNull().NotEmpty();
        }

        internal class PingWithIntResponseValidator : AbstractValidator<TestRequests.PingWithIntResponse>
        {
            public PingWithIntResponseValidator() => RuleFor(x => x.Message).NotNull().NotEmpty();
        }

        internal class PingWithNoResponseValidator : AbstractValidator<TestRequests.PingWithNoResponse>
        {
            public PingWithNoResponseValidator() => RuleFor(x => x.Message).NotNull().NotEmpty();
        }

        internal class PingWithResultValidator : AbstractValidator<TestRequests.PingWithResult>
        {
            public PingWithResultValidator() => RuleFor(x => x.Message).NotNull().NotEmpty();
        }

        internal class PingWithPongResultValidator : AbstractValidator<TestRequests.PingWithPongResult>
        {
            public PingWithPongResultValidator() => RuleFor(x => x.Message).NotNull().NotEmpty();
        }
    }

    internal static class TestHandlers
    {
        internal class
            PingWithPongResponseHandler : IRequestHandler<TestRequests.PingWithPongResponse, TestResponses.Pong>
        {
            public Task<TestResponses.Pong> Handle(TestRequests.PingWithPongResponse request,
                CancellationToken cancellationToken) =>
                request.Missed
                    ? throw new TestExceptions.PingException()
                    : Task.FromResult(new TestResponses.Pong($"{request.Message} Pong"));
        }

        internal class
            PingWithIntResponseHandler : IRequestHandler<TestRequests.PingWithIntResponse, int>
        {
            public Task<int> Handle(TestRequests.PingWithIntResponse request,
                CancellationToken cancellationToken) =>
                request.Missed ? throw new TestExceptions.PingException() : Task.FromResult(1);
        }

        internal class PingWithNoResultHandler : IRequestHandler<TestRequests.PingWithNoResponse>
        {
            public Task Handle(TestRequests.PingWithNoResponse request, CancellationToken cancellationToken) =>
                request.Missed ? throw new TestExceptions.PingException() : Task.CompletedTask;
        }

        internal class
            PingWithPongResultHandler : IRequestHandler<TestRequests.PingWithPongResult, Result<TestResponses.Pong>>
        {
            public Task<Result<TestResponses.Pong>> Handle(TestRequests.PingWithPongResult request,
                CancellationToken cancellationToken)
            {
                Result<TestResponses.Pong> result = request.Missed
                    ? new ResultError("Ping.Pong", "Missed")
                    : new TestResponses.Pong($"{request.Message} Pong");
                return result;
            }
        }

        internal class
            PingWithResultHandler : IRequestHandler<TestRequests.PingWithResult, Result<Nothing>>
        {
            public Task<Result<Nothing>> Handle(TestRequests.PingWithResult request,
                CancellationToken cancellationToken)
            {
                Result<Nothing> result = request.Missed
                    ? new ResultError("Ping.Pong", "Missed")
                    : Nothing.Value;
                return result;
            }
        }
    }

    internal static class TestResponses
    {
        internal record Pong(string Message);
    }
}
