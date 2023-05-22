using DrifterApps.Seeds.Domain;
using DrifterApps.Seeds.Testing;
using MediatR;
using MediatR.Pipeline;
using NSubstitute;
using Xunit.Categories;

namespace DrifterApps.Seeds.Application.Mediatr.Tests;

[UnitTest]
public class UnitOfWorkExceptionHandlerTests
{
    private readonly Driver _driver = new();

    [Fact]
    public async Task GivenHandle_WhenUnitOfWorkValid_ThenRollbackWorkIsCalled()
    {
        // arrange
        var sut = _driver.Build();

        // act
#pragma warning disable S3928
        await sut.Handle(_driver.Request, new ArgumentException(), _driver.State, default);
#pragma warning restore S3928

        // assert
        _driver.ShouldHaveCalledRollbackWork();
    }

    private class Driver : IDriverOf<UnitOfWorkExceptionHandler<SampleRequest, SampleResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        public SampleRequest Request { get; } = new();

        public SampleResponse Response { get; } = new();

        public RequestExceptionHandlerState<SampleResponse> State { get; } = new();

        public UnitOfWorkExceptionHandler<SampleRequest, SampleResponse> Build() => new(_unitOfWork);

        public Driver ShouldHaveCalledRollbackWork()
        {
            _unitOfWork.Received().RollbackWorkAsync(Arg.Any<CancellationToken>());

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
