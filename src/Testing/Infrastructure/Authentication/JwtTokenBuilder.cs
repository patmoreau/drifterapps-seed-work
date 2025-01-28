using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DrifterApps.Seeds.Testing.Infrastructure.Authentication;

/// <summary>
///     A builder class for creating JWT tokens with customizable parameters.
/// </summary>
public sealed class JwtTokenBuilder
{
    private readonly List<Claim> _claims = [];
    private string? _audience;
    private TimeSpan _expiry = TimeSpan.FromHours(1);
    private string? _issuer;
    private DateTime _notBefore = DateTime.UtcNow;

    /// <summary>
    ///     Sets the audience of the JWT token.
    /// </summary>
    /// <param name="audience">The audience to set.</param>
    /// <returns>The current instance of <see cref="JwtTokenBuilder" />.</returns>
    public JwtTokenBuilder ForAudience(string audience)
    {
        _audience = audience;
        return this;
    }

    /// <summary>
    ///     Sets the issuer of the JWT token.
    /// </summary>
    /// <param name="issuer">The issuer to set.</param>
    /// <returns>The current instance of <see cref="JwtTokenBuilder" />.</returns>
    public JwtTokenBuilder IssuedBy(string issuer)
    {
        _issuer = issuer;
        return this;
    }

    /// <summary>
    ///     Sets the date time validity of the JWT token.
    /// </summary>
    /// <param name="notBefore">DateTime at which the token will be valid.</param>
    /// <returns>The current instance of <see cref="JwtTokenBuilder" />.</returns>
    public JwtTokenBuilder ValidOn(DateTime notBefore)
    {
        _notBefore = notBefore;
        return this;
    }

    /// <summary>
    ///     Sets the expiry duration of the JWT token.
    /// </summary>
    /// <param name="expiry">The expiry duration to set.</param>
    /// <returns>The current instance of <see cref="JwtTokenBuilder" />.</returns>
    public JwtTokenBuilder ExpireIn(TimeSpan expiry)
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
    ///     Adds a scope claims to the JWT token.
    /// </summary>
    /// <param name="scopes">The scopes to add.</param>
    /// <returns>The current instance of <see cref="JwtTokenBuilder" />.</returns>
    public JwtTokenBuilder WithScopes(params string[] scopes)
    {
        ArgumentNullException.ThrowIfNull(scopes);

        foreach (var scope in scopes)
        {
            _claims.Add(new Claim("scope", scope));
        }

        return this;
    }

    /// <summary>
    ///     Builds the JWT token with the specified parameters.
    /// </summary>
    /// <returns>The generated JWT token as a string.</returns>
    public string Build()
    {
        ArgumentNullException.ThrowIfNull(_issuer);
        ArgumentNullException.ThrowIfNull(_audience);

        var expires = _notBefore.Add(_expiry);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            _claims,
            _notBefore,
            expires,
            JwtSigningCredentials.SigningCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record SigningKeyInfo(string Modulus, string Exponent, string Kid, string Algorithm);
