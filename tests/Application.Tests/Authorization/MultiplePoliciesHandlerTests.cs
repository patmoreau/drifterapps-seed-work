using System.Security.Claims;
using DrifterApps.Seeds.Application.Authorization;
using DrifterApps.Seeds.Testing;
using Microsoft.AspNetCore.Authorization;
using NSubstitute;

namespace DrifterApps.Seeds.Application.Tests.Authorization;

[UnitTest]
public class MultiplePoliciesHandlerTests
{
    private readonly Driver _driver = new();

    [Fact]
    public async Task GivenHandleRequirementAsync_WhenAllPoliciesSatisfied_ThenSucceed()
    {
        // Arrange
        var sut = _driver
            .GivenAllRequiredPolicies(out var context)
            .WhenAllPolicyPassing()
            .Build();

        // Act
        await sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task GivenHandleRequirementAsync_WhenAnyPolicySatisfied_ThenSucceed()
    {
        // Arrange
        var sut = _driver
            .GivenAnyRequiredPolicies(out var context)
            .WhenSomePolicyPassing()
            .Build();

        // Act
        await sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task GivenHandleRequirementAsync_WhenNoPoliciesSatisfied_ThenFail()
    {
        // Arrange
        var sut = _driver
            .GivenAnyRequiredPolicies(out var context)
            .WhenAllPolicyFailing()
            .Build();

        // Act
        await sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    private class Driver : IDriverOf<MultiplePoliciesHandler>
    {
        public const string Policy1 = nameof(Policy1);
        public const string Policy2 = nameof(Policy2);

        private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();
        private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

        private readonly ClaimsPrincipal _user = new(new ClaimsIdentity());

        public MultiplePoliciesHandler Build()
        {
            _serviceProvider.GetService(Arg.Is(typeof(IAuthorizationService))).Returns(_authorizationService);
            return new MultiplePoliciesHandler(_serviceProvider);
        }

        public Driver GivenAllRequiredPolicies(out AuthorizationHandlerContext context)
        {
            var requirement = MultiplePoliciesRequirement.ForAllOf(Policy1, Policy2);
            context = new AuthorizationHandlerContext([requirement], _user, null);
            return this;
        }

        public Driver GivenAnyRequiredPolicies(out AuthorizationHandlerContext context)
        {
            var requirement = MultiplePoliciesRequirement.ForAnyOf(Policy1, Policy2);
            context = new AuthorizationHandlerContext([requirement], _user, null);
            return this;
        }

        public Driver WhenAllPolicyPassing()
        {
            _authorizationService.AuthorizeAsync(Arg.Is(_user), Arg.Any<object?>(), Arg.Is(Policy1))
                .Returns(AuthorizationResult.Success());
            _authorizationService.AuthorizeAsync(Arg.Is(_user), Arg.Any<object?>(), Arg.Is(Policy2))
                .Returns(AuthorizationResult.Success());
            return this;
        }

        public Driver WhenAllPolicyFailing()
        {
            _authorizationService.AuthorizeAsync(Arg.Is(_user), Arg.Any<object?>(), Arg.Is(Policy1))
                .Returns(AuthorizationResult.Failed());
            _authorizationService.AuthorizeAsync(Arg.Is(_user), Arg.Any<object?>(), Arg.Is(Policy2))
                .Returns(AuthorizationResult.Failed());
            return this;
        }

        public Driver WhenSomePolicyPassing()
        {
            _authorizationService.AuthorizeAsync(Arg.Is(_user), Arg.Any<object?>(), Arg.Is(Policy1))
                .Returns(AuthorizationResult.Failed());
            _authorizationService.AuthorizeAsync(Arg.Is(_user), Arg.Any<object?>(), Arg.Is(Policy2))
                .Returns(AuthorizationResult.Success());
            return this;
        }
    }
}
