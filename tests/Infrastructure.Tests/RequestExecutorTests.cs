using System.Text.Json;
using System.Text.Json.Serialization;
using DrifterApps.Seeds.Application.Converters;
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
        private readonly JsonSerializerOptions _options;

        private readonly IJsonSerializerOptionsFactory _jsonSerializerOptionsFactory =
            Substitute.For<IJsonSerializerOptionsFactory>();

        private readonly ILogger<RequestExecutor> _logger = Substitute.For<ILogger<RequestExecutor>>();
        private readonly IMediator _mediator = Substitute.For<IMediator>();

        public Driver()
        {
            _options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            };
            _options.Converters.Add(new StronglyTypedIdJsonConverterFactory());
            _jsonSerializerOptionsFactory.CreateOptions().Returns(_options);
            SerializedObject  =
                MediatorSerializedObject.SerializeObject(new SampleRequest(), nameof(SampleRequest), _options);
        }

        public MediatorSerializedObject SerializedObject { get; private set; }

        public RequestExecutor Build() => new(_jsonSerializerOptionsFactory, _mediator, _logger);

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
