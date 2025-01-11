using Bogus;
using DrifterApps.Seeds.Testing.Infrastructure.Persistence;

namespace DrifterApps.Seeds.Testing.Tests.Infrastructure.Persistence;

[UnitTest]
public class MariaDatabaseServerTests
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task GivenCreateServer_WhenBuilt_ThenContainerShouldBeCreated()
    {
        // Arrange
        var databaseName = _faker.Database.Engine();
        var container = MariaDatabaseServer.CreateServer(databaseName);

        // Act
        var action = () => container.StartAsync();

        // Assert
        await action.Should().NotThrowAsync();
        await container.DisposeAsync();
    }

    [Fact]
    public async Task GivenGetConnectionString_WhenBuilt_ThenContainerShouldBeCreated()
    {
        // Arrange
        var databaseName = _faker.Database.Engine();
        var port = _faker.Internet.Port();
        var container = MariaDatabaseServer.CreateServer(databaseName, port);
        await container.StartAsync();

        // Act
        var result = container.ConnectionString;

        // Assert
        result.Should().ContainAll(databaseName);
        await container.DisposeAsync();
    }
}
