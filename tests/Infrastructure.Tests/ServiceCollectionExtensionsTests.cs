using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tests;

[UnitTest]
public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void GivenAddHangfireRequestScheduler_WhenInvoked_ThenServicesRegistered()
    {
        // arrange
        var serviceCollection = new ServiceCollection();

        // act
        serviceCollection.AddHangfireRequestScheduler();

        // asset
        serviceCollection.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IRequestExecutor) &&
                descriptor.ImplementationType == typeof(RequestExecutor) &&
                descriptor.Lifetime == ServiceLifetime.Transient)
            .And
            .Contain(descriptor =>
                descriptor.ServiceType == typeof(IRequestScheduler) &&
                descriptor.ImplementationType == typeof(RequestScheduler) &&
                descriptor.Lifetime == ServiceLifetime.Transient);
    }
}
