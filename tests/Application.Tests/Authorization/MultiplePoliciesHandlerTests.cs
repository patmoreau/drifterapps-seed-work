using System.Security.Claims;
using DrifterApps.Seeds.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using NSubstitute;

namespace DrifterApps.Seeds.Application.Tests.Authorization;

[UnitTest]
public class MultiplePoliciesHandlerTests
{
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly MultiplePoliciesHandler _handler;

    public MultiplePoliciesHandlerTests() => _handler = new MultiplePoliciesHandler(_authorizationService);

    [Fact]
    public async Task GivenHandleRequirementAsync_WhenAllPoliciesSatisfied_ThenSucceed()
    {
        // Arrange
        var requirement = MultiplePoliciesRequirement.ForAllOf("Policy1", "Policy2");
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var context = new AuthorizationHandlerContext([requirement], user, null);

        _authorizationService.AuthorizeAsync(Arg.Is(user), Arg.Any<object>(), Arg.Is("Policy1"))
            .Returns(AuthorizationResult.Success());
        _authorizationService.AuthorizeAsync(Arg.Is(user), Arg.Any<object>(), Arg.Is("Policy2"))
            .Returns(AuthorizationResult.Success());

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task GivenHandleRequirementAsync_WhenAnyPolicySatisfied_ThenSucceed()
    {
        // Arrange
        var requirement = MultiplePoliciesRequirement.ForAnyOf("Policy1", "Policy2");
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var context = new AuthorizationHandlerContext([requirement], user, null);

        _authorizationService.AuthorizeAsync(Arg.Is(user), Arg.Any<object>(), Arg.Is("Policy1"))
            .Returns(AuthorizationResult.Failed());
        _authorizationService.AuthorizeAsync(Arg.Is(user), Arg.Any<object>(), Arg.Is("Policy2"))
            .Returns(AuthorizationResult.Success());

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task GivenHandleRequirementAsync_WhenNoPoliciesSatisfied_ThenFail()
    {
        // Arrange
        var requirement = MultiplePoliciesRequirement.ForAnyOf("Policy1", "Policy2");
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var context = new AuthorizationHandlerContext([requirement], user, null);

        _authorizationService.AuthorizeAsync(Arg.Is(user), Arg.Any<object>(), Arg.Is("Policy1"))
            .Returns(AuthorizationResult.Failed());
        _authorizationService.AuthorizeAsync(Arg.Is(user), Arg.Any<object>(), Arg.Is("Policy2"))
            .Returns(AuthorizationResult.Failed());

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }
}
