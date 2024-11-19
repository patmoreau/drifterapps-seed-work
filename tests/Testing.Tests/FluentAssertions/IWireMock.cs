using Refit;

namespace DrifterApps.Seeds.Testing.Tests.FluentAssertions;

internal interface IWireMock
{
    [Get("/wiremock")]
    Task<IApiResponse> GetAsync();

    [Get("/wiremock")]
    Task<IApiResponse> GetWithMonitoringHeadersAsync([Header("X-Correlation-Id")] Guid correlationId);
}
