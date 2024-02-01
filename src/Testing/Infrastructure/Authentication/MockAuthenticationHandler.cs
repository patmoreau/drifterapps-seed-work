using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DrifterApps.Seeds.Testing.Infrastructure.Authentication;

#if NET7_0
public sealed class MockAuthenticationHandler(
    IOptionsMonitor<MockAuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock)
    : AuthenticationHandler<MockAuthenticationSchemeOptions>(options, logger, encoder, clock)
#elif NET8_0
public sealed class MockAuthenticationHandler(
    IOptionsMonitor<MockAuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<MockAuthenticationSchemeOptions>(options, logger, encoder)
#endif
{
    public const string AuthenticationScheme = "Test";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.TryGetValue("Authorization", out var value))
            return Task.FromResult(AuthenticateResult.NoResult());

        if (!AuthenticationHeaderValue.TryParse(value,
                out var headerValue))
            return Task.FromResult(AuthenticateResult.NoResult());

        if (!AuthenticationScheme.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.NoResult());

        if (headerValue.Parameter is null) return Task.FromResult(AuthenticateResult.NoResult());

        var userId = headerValue.Parameter;

        List<Claim> claims = [new Claim(ClaimTypes.Name, userId), new Claim(ClaimTypes.NameIdentifier, userId)];

        claims.AddRange(Options.ConfigureUserClaims(userId));

        ClaimsIdentity identity = new(claims, AuthenticationScheme);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
