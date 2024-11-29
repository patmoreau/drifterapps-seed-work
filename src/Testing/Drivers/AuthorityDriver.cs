using System.Diagnostics.CodeAnalysis;
using DrifterApps.Seeds.Testing.Infrastructure.Authentication;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace DrifterApps.Seeds.Testing.Drivers;

[SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments")]
public sealed class AuthorityDriver : WireMockDriver
{
    protected override WireMockServer CreateServer() =>
        WireMockServer.StartWithAdminInterface(JwtTokenBuilder.Authority);

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
                    issuer = $"{JwtTokenBuilder.Authority}/",
                    authorization_endpoint = $"{JwtTokenBuilder.Authority}/authorize",
                    token_endpoint = $"{JwtTokenBuilder.Authority}/oauth/token",
                    device_authorization_endpoint = $"{JwtTokenBuilder.Authority}/oauth/device/code",
                    userinfo_endpoint = $"{JwtTokenBuilder.Authority}/userinfo",
                    mfa_challenge_endpoint = $"{JwtTokenBuilder.Authority}/mfa/challenge",
                    jwks_uri = $"{JwtTokenBuilder.Authority}/.well-known/jwks.json",
                    registration_endpoint = $"{JwtTokenBuilder.Authority}/oidc/register",
                    revocation_endpoint = $"{JwtTokenBuilder.Authority}/oauth/revoke",
                    scopes_supported = new[]
                    {
                        "openid", "profile", "offline_access", "name", "given_name", "family_name", "nickname", "email",
                        "email_verified", "picture", "created_at", "identities", "phone", "address"
                    },
                    response_types_supported = new[]
                    {
                        "code", "token", "id_token", "code token", "code id_token", "token id_token",
                        "code token id_token"
                    },
                    code_challenge_methods_supported = new[] {"S256", "plain"},
                    response_modes_supported = new[] {"query", "fragment", "form_post"},
                    subject_types_supported = new[] {"public"},
                    id_token_signing_alg_values_supported = new[] {"HS256", "RS256", "PS256"},
                    token_endpoint_auth_methods_supported = new[]
                        {"client_secret_basic", "client_secret_post", "private_key_jwt"},
                    claims_supported = new[]
                    {
                        "aud", "auth_time", "created_at", "email", "email_verified", "exp", "family_name", "given_name",
                        "iat", "identities", "iss", "name", "nickname", "phone_number", "picture", "sub"
                    },
                    request_uri_parameter_supported = false,
                    request_parameter_supported = false,
                    token_endpoint_auth_signing_alg_values_supported = new[] {"RS256", "RS384", "PS256"},
                    backchannel_logout_supported = true,
                    backchannel_logout_session_supported = true,
                    end_session_endpoint = $"{JwtTokenBuilder.Authority}/oidc/logout"
                })
            );

    private void ConfigureJwks()
    {
        var signingKeyInfo = JwtTokenBuilder.GetSigningKeyInfo;
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
