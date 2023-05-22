using DrifterApps.Seeds.Domain;
using DrifterApps.Seeds.Testing;
using MediatR;
using NSubstitute;
using Xunit.Categories;

namespace DrifterApps.Seeds.Application.Mediatr.Tests;

[UnitTest]
public class UnitOfWorkPostProcessorTests
{
    private readonly Driver _driver = new();

    [Fact]
    public async Task GivenProcess_WhenUnitOfWorkValid_ThenCommitWorkIsCalled()
    {
        // arrange
        var sut = _driver.Build();

        // act
        await sut.Process(_driver.Request, _driver.Response, default);

        // assert
        _driver.ShouldHaveCalledCommitWork();
    }

    private class Driver : IDriverOf<UnitOfWorkPostProcessor<SampleRequest, SampleResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        public SampleRequest Request { get; } = new();

        public SampleResponse Response { get; } = new();

        public UnitOfWorkPostProcessor<SampleRequest, SampleResponse> Build() => new(_unitOfWork);

        public Driver ShouldHaveCalledCommitWork()
        {
            _unitOfWork.Received().CommitWorkAsync(Arg.Any<CancellationToken>());

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
