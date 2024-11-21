using System.Net;
using System.Text.Json;
using Bogus;
using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Tests.FluentAssertions;
using Refit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static DrifterApps.Seeds.Testing.Tests.FluentAssertions.IApiResponseWireMock;

namespace DrifterApps.Seeds.Testing.Tests.Drivers;

public class ApiResponseDriver : WireMockDriver
{
    private static readonly Faker Fake = new();

    private HttpClient? _client;
    private HttpStatusCode? _notStatusCode;

    internal Lazy<IApiResponseWireMock> Api => new(() =>
    {
        if (_client is null)
        {
            _client = Server.CreateClient();
            _client.Timeout = TimeSpan.FromSeconds(5);
        }

        return RestService.For<IApiResponseWireMock>(_client);
    });

    internal Guid CorrelationId { get; } = Fake.Random.Guid();

    internal HttpStatusCode FailedStatusCode { get; } =
        Fake.PickRandom(Enum.GetValues<HttpStatusCode>().Where(x => (int) x is >= 300));

    internal HttpStatusCode SuccessStatusCode { get; } =
        Fake.PickRandom(Enum.GetValues<HttpStatusCode>().Where(x => (int) x is >= 200 and < 300));

    internal HttpStatusCode StatusCode { get; } = Fake.PickRandom<HttpStatusCode>();

    internal HttpStatusCode NotStatusCode =>
        _notStatusCode ??= Fake.PickRandom(Enum.GetValues<HttpStatusCode>().Where(x => x != StatusCode));

    internal HttpStatusCode NotAuthorizedStatusCode { get; } =
        Fake.PickRandom(HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);

    internal HttpStatusCode NotForbiddenStatusCode { get; } =
        Fake.PickRandom(Enum.GetValues<HttpStatusCode>().Where(x => x is not HttpStatusCode.Forbidden));

    internal string ErrorMessage { get; } = Fake.Lorem.Sentence();

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        Configure();
    }

    public override Task DisposeAsync()
    {
        _client?.Dispose();

        return base.DisposeAsync();
    }

    protected override void Configure()
    {
        Server
            .Given(Request.Create().WithPath(WithCorrelationId))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithHeader("X-Correlation-Id", CorrelationId.ToString())
            );

        Server
            .Given(Request.Create().WithPath(WithoutCorrelationId))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.BadRequest)
            );

        Server
            .Given(Request.Create().WithPath(IsSuccessful))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(SuccessStatusCode)
            );

        Server
            .Given(Request.Create().WithPath(IsNotSuccessful))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(FailedStatusCode)
            );

        Server
            .Given(Request.Create().WithPath(IsFailure))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(FailedStatusCode)
            );

        Server
            .Given(Request.Create().WithPath(IsNotFailure))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(SuccessStatusCode)
            );

        Server
            .Given(Request.Create().WithPath(IsAuthorized))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(SuccessStatusCode)
            );

        Server
            .Given(Request.Create().WithPath(IsNotAuthorized))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(NotAuthorizedStatusCode)
            );

        Server
            .Given(Request.Create().WithPath(IsForbidden))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.Forbidden)
            );

        Server
            .Given(Request.Create().WithPath(IsNotForbidden))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(NotForbiddenStatusCode)
            );

        Server
            .Given(Request.Create().WithPath(IsUnauthorized))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.Unauthorized)
            );

        Server
            .Given(Request.Create().WithPath(IsNotUnauthorized))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(SuccessStatusCode)
            );

        Server
            .Given(Request.Create().WithPath(IsWithStatusCode))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(StatusCode)
            );

        Server
            .Given(Request.Create().WithPath(IsNotWithStatusCode))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(NotStatusCode)
            );

        Server
            .Given(Request.Create().WithPath(IsWithError))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithBody(JsonSerializer.Serialize(new ProblemDetails
                    {
                        Detail = ErrorMessage
                    }))
            );
    }
}
