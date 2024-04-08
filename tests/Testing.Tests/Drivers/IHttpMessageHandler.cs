namespace DrifterApps.Seeds.Testing.Tests.Drivers;

internal interface IHttpMessageHandler
{
    Task<HttpResponseMessage> MockSendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
}
