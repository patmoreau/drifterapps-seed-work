using DrifterApps.Seeds.Testing.Drivers;

namespace DrifterApps.Seeds.Testing.Tests.Drivers;

[UnitTest]
public class AuthorityDriverTests(AuthorityDriver driver) : IClassFixture<AuthorityDriver>
{
    [Fact]
    public void GivenConstructor_WhenCalled_ThenReturnInstance() =>
        // Arrange
        // Act
        // Assert
        _ = driver.Should().NotBeNull().And.Subject.As<AuthorityDriver>().Server.IsStarted.Should().BeTrue();
}
