using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DrifterApps.Seeds.Testing.Infrastructure.Authentication;

/// <summary>
///     A builder class for creating JWT tokens with customizable parameters.
/// </summary>
public sealed partial class JwtTokenBuilder
{
    /// <summary>
    ///     The authority domain for the JWT token issuer.
    /// </summary>
    public const string AuthorityDomain = "localhost:9096";

    private readonly List<Claim> _claims = [];
    private string _audience = "default-audience";
    private TimeSpan _expiry = TimeSpan.FromHours(1);

    /// <summary>
    ///     Sets the audience of the JWT token.
    /// </summary>
    /// <param name="audience">The audience to set.</param>
    /// <returns>The current instance of <see cref="JwtTokenBuilder" />.</returns>
    public JwtTokenBuilder WithAudience(string audience)
    {
        _audience = audience;
        return this;
    }

    /// <summary>
    ///     Sets the expiry duration of the JWT token.
    /// </summary>
    /// <param name="expiry">The expiry duration to set.</param>
    /// <returns>The current instance of <see cref="JwtTokenBuilder" />.</returns>
    public JwtTokenBuilder WithExpiry(TimeSpan expiry)
    {
        _expiry = expiry;
        return this;
    }

    /// <summary>
    ///     Adds a claim to the JWT token.
    /// </summary>
    /// <param name="type">The type of the claim.</param>
    /// <param name="value">The value of the claim.</param>
    /// <returns>The current instance of <see cref="JwtTokenBuilder" />.</returns>
    public JwtTokenBuilder WithClaim(string type, string value)
    {
        _claims.Add(new Claim(type, value));
        return this;
    }

    /// <summary>
    ///     Adds a role claim to the JWT token.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <returns>The current instance of <see cref="JwtTokenBuilder" />.</returns>
    public JwtTokenBuilder WithRole(string role)
    {
        _claims.Add(new Claim(ClaimTypes.Role, role));
        return this;
    }

    /// <summary>
    ///     Builds the JWT token with the specified parameters.
    /// </summary>
    /// <returns>The generated JWT token as a string.</returns>
    public string Build()
    {
        var token = new JwtSecurityToken(
            $"{Authority}/",
            _audience,
            _claims,
            expires: DateTime.UtcNow.Add(_expiry),
            signingCredentials: SigningCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record SigningKeyInfo(string Modulus, string Exponent, string Kid, string Algorithm);
