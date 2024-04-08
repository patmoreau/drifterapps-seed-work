namespace DrifterApps.Seeds.Testing.Tests.Drivers;

internal class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly IHttpMessageHandler _httpMessageHandler;

    internal MockHttpMessageHandler(IHttpMessageHandler httpMessageHandler) => _httpMessageHandler = httpMessageHandler;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken) => _httpMessageHandler.MockSendAsync(request, cancellationToken);
}
