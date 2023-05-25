using DrifterApps.Seeds.Domain;
using DrifterApps.Seeds.Testing;
using DrifterApps.Seeds.Testing.Attributes;
using FluentAssertions.Execution;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Categories;

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
    public void GivenPing_WhenNoFirstMessage_ThenValidatorIsCalledAndThrowsValidationException()
    {
        // arrange
        var mediator = _driver.GivenMediatrIsConfigured()
            .GivenPingPongGameIsReady()
            .Build();

        // act
        Func<Task> action = async () => await mediator.Send(new Ping(string.Empty, false));

        // assert
        using var scope = new AssertionScope();
        action.Should().ThrowAsync<ValidationException>();
        _driver.ShouldNotHaveCalledUnitOfWorkBeginWorkAsync();
    }

    [Fact]
    public void GivenPing_WhenMissed_ThenRollback()
    {
        // arrange
        var mediator = _driver.GivenMediatrIsConfigured()
            .GivenPingPongGameIsReady()
            .Build();

        // act
        Func<Task> action = async () => await mediator.Send(new Ping(nameof(Ping), true));

        // assert
        using var scope = new AssertionScope();
        action.Should().ThrowAsync<PingException>();
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
        var pong = await mediator.Send(new Ping(nameof(Ping), false));

        // assert
        using var scope = new AssertionScope();
        pong.Should().NotBeNull().And.BeEquivalentTo(new Pong("Ping Pong"));
        _driver
            .ShouldHaveCalledUnitOfWorkBeginWorkAsync()
            .ShouldHaveCalledUnitOfWorkCommitWorkAsync()
            .ShouldNotHaveCalledUnitOfWorkRollbackWorkAsync();
    }

    private class Driver : IDriverOf<IMediator>, IAsyncDisposable
    {
        private readonly ILogger<Ping> _logger;
        private readonly ServiceCollection _serviceCollection;
        private readonly IUnitOfWork _unitOfWork;
        private ServiceProvider? _serviceProvider;

        public Driver()
        {
            _serviceCollection = new ServiceCollection();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _logger = Substitute.For<ILogger<Ping>>();
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
            _serviceCollection.AddTransient<ILogger<Ping>>(_ => _logger);
            _serviceCollection.AddTransient<IValidator<Ping>, PingValidator>();

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
                serviceDescriptor.ServiceType == typeof(IRequestExceptionHandler<,,>) &&
                serviceDescriptor.ImplementationType == typeof(UnitOfWorkExceptionHandler<,,>) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient);
            _serviceCollection.Should().Contain(serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(IRequestPostProcessor<,>) &&
                serviceDescriptor.ImplementationType == typeof(UnitOfWorkPostProcessor<,>) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient);
            _serviceCollection.Should().Contain(serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(IRequestPreProcessor<>) &&
                serviceDescriptor.ImplementationType == typeof(UnitOfWorkPreProcessor<>) &&
                serviceDescriptor.Lifetime == ServiceLifetime.Transient);
            _serviceCollection.Should().Contain(serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(IRequestPreProcessor<>) &&
                serviceDescriptor.ImplementationType == typeof(ValidationPreProcessor<>) &&
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

    internal record Ping(string Message, bool Missed) : IRequest<Pong>, IUnitOfWorkRequest;

#pragma warning disable CA1032, CA1064
    internal class PingException : Exception
    {
    }
#pragma warning restore CA1032, CA1064

    internal class PingValidator : AbstractValidator<Ping>
    {
        public PingValidator() => RuleFor(x => x.Message).NotNull().NotEmpty();
    }

    internal class PingHandler : IRequestHandler<Ping, Pong>
    {
        public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
        {
            if (request.Missed) throw new PingException();

            return Task.FromResult(new Pong($"{request.Message} Pong"));
        }
    }

    internal record Pong(string Message);
}
