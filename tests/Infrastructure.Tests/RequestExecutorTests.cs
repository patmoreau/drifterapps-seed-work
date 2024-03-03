using DrifterApps.Seeds.Infrastructure;
using DrifterApps.Seeds.Testing;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Infrastructure.Tests;

[UnitTest]
public class RequestExecutorTests
{
    private readonly Driver _driver = new();

    [Fact]
    public async Task GivenExecuteCommand_WhenSerializedObjectNotValid_ThenDoNothing()
    {
        // arrange
        var sut = _driver.GivenSerializedObjectNotOfTypeIBaseRequest().Build();

        // act
        await sut.ExecuteCommandAsync(_driver.SerializedObject);

        // assert
        _driver.ShouldNotHaveSentUsingMediator();
    }

    [Fact]
    public async Task GivenExecuteCommand_WhenSerializedObjectIsBaseRequest_ThenSend()
    {
        // arrange
        var sut = _driver.Build();

        // act
        await sut.ExecuteCommandAsync(_driver.SerializedObject);

        // assert
        _driver.ShouldHaveSentUsingMediator();
    }

    private class Driver : IDriverOf<RequestExecutor>
    {
        private readonly ILogger<RequestExecutor> _logger = Substitute.For<ILogger<RequestExecutor>>();
        private readonly IMediator _mediator = Substitute.For<IMediator>();

        public MediatorSerializedObject SerializedObject { get; private set; } =
            MediatorSerializedObject.SerializeObject(new SampleRequest(), nameof(SampleRequest));

        public RequestExecutor Build() => new(_mediator, _logger);

        public Driver GivenSerializedObjectNotOfTypeIBaseRequest()
        {
            SerializedObject =
                new MediatorSerializedObject(typeof(string).AssemblyQualifiedName!, string.Empty, string.Empty);
            return this;
        }

        public Driver ShouldNotHaveSentUsingMediator()
        {
            _mediator.Received(0).Send(Arg.Any<IBaseRequest>());

            return this;
        }

        public Driver ShouldHaveSentUsingMediator()
        {
            _mediator.Received(1).Send(Arg.Any<IBaseRequest>());

            return this;
        }
    }

    private class SampleRequest : IBaseRequest
    {
        public string Property1 { get; } = null!;
        public int Property2 { get; }
    }
}
