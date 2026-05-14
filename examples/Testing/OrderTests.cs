using Bogus;
using DrifterApps.Seeds.Testing;
using DrifterApps.Seeds.Testing.Drivers;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using SampleApp.Domain;
using System.Net.Http.Headers;
using Xunit;

namespace SampleApp.Tests;

// ── Fake builder ─────────────────────────────────────────────────────────────

public class FakeOrderBuilder : FakerBuilder<Order>
{
    protected override Faker<Order> Faker => CreateUninitializedFaker<Order>()
        .RuleFor(o => o.Id, _ => OrderId.New)
        .RuleFor(o => o.CustomerId, _ => CustomerId.New)
        .RuleFor(o => o.Total, f => f.Finance.Amount(1, 10_000))
        .RuleFor(o => o.CreatedAt, f => f.Date.PastOffset())
        .RuleFor(o => o.Status, _ => OrderStatus.Pending);

    public FakeOrderBuilder WithCustomer(CustomerId customerId)
    {
        Faker.RuleFor(o => o.CustomerId, _ => customerId);
        return this;
    }
}

// ── Unit test ─────────────────────────────────────────────────────────────────

[UnitTest]
public class OrderTests
{
    [Fact]
    public void Create_SetsStatusToPending()
    {
        var customerId = CustomerId.New;

        var order = Order.Create(customerId, 99.99m);

        order.Status.Should().Be(OrderStatus.Pending);
        order.CustomerId.Should().Be(customerId);
        order.Total.Should().Be(99.99m);
    }

    [Fact]
    public void Ship_TransitionsToShipped()
    {
        var order = new FakeOrderBuilder().Build();
        order.Ship();
        order.Status.Should().Be(OrderStatus.Shipped);
    }

    [Fact]
    public void Ship_WhenAlreadyShipped_Throws()
    {
        var order = new FakeOrderBuilder().Build();
        order.Ship();

        var act = () => order.Ship();

        act.Should().Throw<InvalidOperationException>();
    }
}

// ── Integration test with mock authority ─────────────────────────────────────

[EndToEndTest]
public class OrderApiTests : IAsyncLifetime
{
    private readonly AuthorityDriver _authority = new();
    private HttpClient _client = null!;

    public async ValueTask InitializeAsync()
    {
        await _authority.InitializeAsync();

        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.ConfigureAppConfiguration((_, cfg) =>
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:Authority"] = _authority.Authority.ToString(),
                    ["Authentication:Audience"] = "sample-api"
                })));

        _client = factory.CreateClient();
    }

    public async ValueTask DisposeAsync() => await _authority.DisposeAsync();

    [Fact]
    public async Task GetOrders_WithValidToken_Returns200()
    {
        var token = new JwtTokenBuilder()
            .ForAudience("sample-api")
            .IssuedBy(_authority.Authority.ToString())
            .ExpireIn(TimeSpan.FromMinutes(5))
            .WithRole("OrderManager")
            .Build();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/orders?offset=0&limit=10");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOrders_WithoutToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/orders");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}
