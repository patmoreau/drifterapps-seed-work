using DrifterApps.Seeds.Domain;
using DrifterApps.Seeds.Testing;
using MediatR;
using NSubstitute;
using Xunit.Categories;

namespace DrifterApps.Seeds.Application.Mediatr.Tests;

[UnitTest]
public class UnitOfWorkPreProcessorTests
{
    private readonly Driver _driver = new();

    [Fact]
    public async Task GivenProcess_WhenUnitOfWorkValid_ThenBeginWorkIsCalled()
    {
        // arrange
        var sut = _driver.Build();

        // act
        await sut.Process(_driver.Request, default);

        // assert
        _driver.ShouldHaveCalledBeginWork();
    }

    private class Driver : IDriverOf<UnitOfWorkPreProcessor<SampleRequest>>
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        public SampleRequest Request { get; } = new();

        public UnitOfWorkPreProcessor<SampleRequest> Build() => new(_unitOfWork);

        public Driver ShouldHaveCalledBeginWork()
        {
            _unitOfWork.Received().BeginWorkAsync(Arg.Any<CancellationToken>());

            return this;
        }
    }

    private sealed class SampleRequest : IRequest<SampleResponse>, IUnitOfWorkRequest
    {
        public string Name { get; set; } = null!;
        public int Age { get; set; }
    }

    private sealed class SampleResponse
    {
    }
}
