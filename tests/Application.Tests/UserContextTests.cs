using System.Security.Claims;
using DrifterApps.Seeds.Testing;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace DrifterApps.Seeds.Application.Tests;

[UnitTest]
public class UserContextTests
{
    private readonly UserContextDriver _driver = new();

    [Fact]
    public void GivenGetUserId_WhenUserHasNameIdentifierClaim_ThenReturnNameIdentifierClaim()
    {
        // arrange
        var sut = _driver.WhenUserHasNameIdentifierClaim(out var idNameIdentifier).Build();

        // act
        var result = sut.IdentityObjectId;

        // assert
        result.Should().Be(idNameIdentifier);
    }

    [Fact]
    public void GivenGetUserId_WhenUserHasSubClaim_ThenReturnSubClaim()
    {
        // arrange
        var sut = _driver.WhenUserHasSubClaim(out var idSub).Build();

        // act
        var result = sut.IdentityObjectId;

        // assert
        result.Should().Be(idSub);
    }

    [Fact]
    public void GivenGetUserId_WhenUserHasNameIdentifierAndSubClaim_ThenReturnNameIdentifierClaim()
    {
        // arrange
        var sut = _driver.WhenUserHasNameIdentifierAndSubClaim(out var idNameIdentifier).Build();

        // act
        var result = sut.IdentityObjectId;

        // assert
        result.Should().Be(idNameIdentifier);
    }

    [Fact]
    public void GivenGetUserId_WhenHttpContextIsNull_ThenReturnEmptyGuid()
    {
        // arrange
        var sut = _driver.WhenHttpContextIsNull().Build();

        // act
        var result = sut.IdentityObjectId;

        // assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void GivenGetUserId_WhenUserHasNoClaims_ThenReturnEmptyGuid()
    {
        // arrange
        var sut = _driver.WhenUserHasNoClaims().Build();

        // act
        var result = sut.IdentityObjectId;

        // assert
        result.Should().Be(string.Empty);
    }

    private class UserContextDriver : IDriverOf<HttpUserContext>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        private readonly string _idNameIdentifier;
        private readonly string _idSub;

        public UserContextDriver()
        {
            _idNameIdentifier = Fakerizer.Random.Hash();
            _idSub = Fakerizer.Random.Hash();
        }

        public HttpUserContext Build() => new(_httpContextAccessor);

        public UserContextDriver WhenUserHasNameIdentifierClaim(out string idNameIdentifier)
        {
            _httpContextAccessor.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, _idNameIdentifier)
                ]))
            };
            idNameIdentifier = _idNameIdentifier;

            return this;
        }

        public UserContextDriver WhenUserHasSubClaim(out string idSub)
        {
            _httpContextAccessor.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim("sub", _idSub)
                ]))
            };
            idSub = _idSub;

            return this;
        }

        public UserContextDriver WhenUserHasNameIdentifierAndSubClaim(out string idNameIdentifier)
        {
            _httpContextAccessor.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, _idNameIdentifier),
                    new Claim("sub", _idSub)
                ]))
            };
            idNameIdentifier = _idNameIdentifier;

            return this;
        }

        public UserContextDriver WhenHttpContextIsNull()
        {
            _httpContextAccessor.HttpContext = null;

            return this;
        }

        public UserContextDriver WhenUserHasNoClaims()
        {
            _httpContextAccessor.HttpContext =
                new DefaultHttpContext {User = new ClaimsPrincipal(new ClaimsIdentity())};

            return this;
        }
    }
}
