using DrifterApps.Seeds.Testing.Infrastructure.Authentication;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace DrifterApps.Seeds.Testing.Drivers;

public sealed class AuthorityDriver : WireMockDriver
{
    /// <summary>
    ///     Gets the authority URL for the token issuer.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when server has not been initialized.</exception>
    public Uri Authority => new(Server.Url ?? throw new InvalidOperationException("Server is not initialized"));

    protected override WireMockServer CreateServer() => WireMockServer.StartWithAdminInterface(useSSL: true);

    protected override void Configure()
    {
        ConfigureOpenIdConfiguration();
        ConfigureJwks();
    }

    private void ConfigureOpenIdConfiguration() =>
        Server
            .Given(Request.Create()
                .UsingMethod("GET")
                .WithPath("/.well-known/openid-configuration"))
            .AtPriority(1)
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithBodyAsJson(new
                {
                    issuer = $"{Authority}",
                    authorization_endpoint = $"{Authority}authorize",
                    token_endpoint = $"{Authority}oauth/token",
                    device_authorization_endpoint = $"{Authority}oauth/device/code",
                    userinfo_endpoint = $"{Authority}userinfo",
                    mfa_challenge_endpoint = $"{Authority}mfa/challenge",
                    jwks_uri = $"{Authority}.well-known/jwks.json",
                    registration_endpoint = $"{Authority}oidc/register",
                    revocation_endpoint = $"{Authority}oauth/revoke",
                    end_session_endpoint = $"{Authority}oidc/logout",
                    scopes_supported = (string[])
                    [
                        "openid", "profile", "offline_access", "name", "given_name", "family_name", "nickname", "email",
                        "email_verified", "picture", "created_at", "identities", "phone", "address"
                    ],
                    response_types_supported = (string[])
                    [
                        "code", "token", "id_token", "code token", "code id_token", "token id_token",
                        "code token id_token"
                    ],
                    code_challenge_methods_supported = (string[]) ["S256", "plain"],
                    response_modes_supported = (string[]) ["query", "fragment", "form_post"],
                    subject_types_supported = (string[]) ["public"],
                    id_token_signing_alg_values_supported = (string[]) ["HS256", "RS256", "PS256"],
                    token_endpoint_auth_methods_supported = (string[])
                        ["client_secret_basic", "client_secret_post", "private_key_jwt"],
                    claims_supported = (string[])
                    [
                        "aud", "auth_time", "created_at", "email", "email_verified", "exp", "family_name", "given_name",
                        "iat", "identities", "iss", "name", "nickname", "phone_number", "picture", "sub"
                    ],
                    request_uri_parameter_supported = false,
                    request_parameter_supported = false,
                    token_endpoint_auth_signing_alg_values_supported = (string[]) ["RS256", "RS384", "PS256"],
                    backchannel_logout_supported = true,
                    backchannel_logout_session_supported = true
                })
            );

    private void ConfigureJwks()
    {
        var signingKeyInfo = JwtSigningCredentials.GetSigningKeyInfo;
        Server
            .Given(Request.Create()
                .UsingMethod("GET")
                .WithPath("/.well-known/jwks.json"))
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithBodyAsJson(new
                {
                    keys = new[]
                    {
                        new
                        {
                            kty = "RSA",
                            use = "sig",
                            n = signingKeyInfo.Modulus,
                            e = signingKeyInfo.Exponent,
                            kid = signingKeyInfo.Kid,
                            alg = signingKeyInfo.Algorithm
                        }
                    }
                })
            );
    }
}
