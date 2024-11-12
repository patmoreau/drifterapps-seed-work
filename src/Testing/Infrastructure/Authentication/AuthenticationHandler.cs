using System.Net.Http.Headers;
using DrifterApps.Seeds.Testing.Drivers;

namespace DrifterApps.Seeds.Testing.Infrastructure.Authentication;

public class AuthenticationHandler(UserDriver userDriver) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Headers.Authorization = userDriver.IsAuthenticated
            ? new AuthenticationHeaderValue("Bearer", userDriver.AuthenticatedUser)
            : null;
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
