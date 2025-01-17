using System.Net;
using System.Text.Json;
using Bogus;
using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Tests.Fakes;
using DrifterApps.Seeds.Testing.Tests.FluentAssertions;
using Refit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static DrifterApps.Seeds.Testing.Tests.FluentAssertions.IApiResponseWireMock;

namespace DrifterApps.Seeds.Testing.Tests.Drivers;

public sealed class ApiResponseDriver : WireMockDriver
{
    private static readonly Faker Fake = new();

#if NET9_0_OR_GREATER
    private static readonly Lock PadLock = new();
#else
    private static readonly object PadLock = new();
#endif

    private static HttpClient? _client;
    private HttpStatusCode? _notStatusCode;

    private readonly RefitSettings? _refitSettings = new()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        })
    };

    internal Lazy<IApiResponseWireMock> Api => new(() =>
    {
        lock (PadLock)
        {
            if (_client is null)
            {
                _client = Server.CreateClient();
                _client.Timeout = TimeSpan.FromSeconds(30);
            }
        }

        return RestService.For<IApiResponseWireMock>(_client, _refitSettings);
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

    internal Guid Content { get; } = Fake.Random.Guid();

    internal IList<FakeClass> EquivalentContent { get; } = [.. new FakeClassBuilder().BuildCollection()];

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

        Server
            .Given(Request.Create().WithPath(IsWithContent))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBody(JsonSerializer.Serialize(Content))
            );

        Server
            .Given(Request.Create().WithPath(IsWithNoContent))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
            );

        Server
            .Given(Request.Create().WithPath(IsWithEquivalentContent))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBody(JsonSerializer.Serialize(EquivalentContent))
            );

        Server
            .Given(Request.Create().WithPath(IsWithNoEquivalentContent))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
            );
    }
}
