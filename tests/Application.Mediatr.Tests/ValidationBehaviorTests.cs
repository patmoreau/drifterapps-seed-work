using DrifterApps.Seeds.FluentResult;
using DrifterApps.Seeds.Testing;
using FluentValidation;
using MediatR;

namespace DrifterApps.Seeds.Application.Mediatr.Tests;

[UnitTest]
public class ValidationBehaviorTests
{
    private const string DelegateSuccess = "Delegate.Success";
    private readonly Driver _driver = new();

    [Fact]
    public async Task GivenHandle_WhenNoValidators_ThenReturnDelegateResult()
    {
        // arrange
        var sut = _driver.WithNoValidators().WithRequestDelegateReturningSuccess(out var next).Build();

        // act
        var result = await sut.Handle(new SampleRequest(), next, CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(DelegateSuccess);
    }

    [Fact]
    public async Task GivenHandle_WhenValidRequest_ThenReturnDelegateResult()
    {
        // arrange
        var sut = _driver.WithSuccessValidator().WithRequestDelegateReturningSuccess(out var next).Build();

        // act
        var result = await sut.Handle(new SampleRequest {Name = "John Doe", Age = 30}, next,
            CancellationToken.None);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(DelegateSuccess);
    }

    [Fact]
    public async Task GivenHandle_WhenInvalidRequest_ThenShouldReturnResultError()
    {
        // arrange
        var sut = _driver.WithSuccessValidator().WithRequestDelegateReturningSuccess(out var next).Build();

        // act
        var result = await sut.Handle(new SampleRequest {Name = string.Empty, Age = -5}, next,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ResultErrorAggregate>();
        result.Error.As<ResultErrorAggregate>().Errors.Should().HaveCount(2);
    }

    private class Driver : IDriverOf<ValidationBehavior<SampleRequest, Result<string>>>
    {
        private List<IValidator<SampleRequest>> _validators = [new SampleRequestValidator()];
        public ValidationBehavior<SampleRequest, Result<string>> Build() => new(_validators);

        public Driver WithNoValidators()
        {
            _validators = [];
            return this;
        }

        public Driver WithSuccessValidator()
        {
            _validators = [new SampleRequestValidator()];
            return this;
        }

        public Driver WithRequestDelegateReturningSuccess(out RequestHandlerDelegate<Result<string>> next)
        {
            next = () => Task.FromResult(Result<string>.Success(DelegateSuccess));
            return this;
        }
    }

    public class SampleRequest : IRequest<Result<string>>
    {
        public string Name { get; set; } = null!;
        public int Age { get; set; }
    }

    public class SampleRequestValidator : AbstractValidator<SampleRequest>
    {
        public SampleRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Age).GreaterThanOrEqualTo(0);
        }
    }
}
