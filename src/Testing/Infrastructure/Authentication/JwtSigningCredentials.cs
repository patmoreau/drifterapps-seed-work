using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace DrifterApps.Seeds.Testing.Infrastructure.Authentication;

internal static class JwtSigningCredentials
{
    private const string SignatureAlgorithm = SecurityAlgorithms.RsaSha256;
    private static readonly string Kid = Guid.NewGuid().ToString("N");
    private static readonly RSA Rsa = RSA.Create(2048);

    static JwtSigningCredentials()
    {
        var keyParameters = Rsa.ExportParameters(false);
        var modulus = Base64UrlEncode(keyParameters.Modulus!);
        var exponent = Base64UrlEncode(keyParameters.Exponent!);
        GetSigningKeyInfo = new SigningKeyInfo(modulus, exponent, Kid, SignatureAlgorithm);
    }

    /// <summary>
    ///     Gets the signing key information.
    /// </summary>
    internal static SigningKeyInfo GetSigningKeyInfo { get; }

    /// <summary>
    ///     Gets the signing credentials for the JWT token.
    /// </summary>
    internal static SigningCredentials SigningCredentials { get; } =
        new(new RsaSecurityKey(Rsa) {KeyId = Kid}, SignatureAlgorithm);

    private static string Base64UrlEncode(byte[] arg)
    {
        var result = Convert.ToBase64String(arg);
        result = result.Split('=')[0];
        result = result.Replace('+', '-');
        result = result.Replace('/', '_');
        return result;
    }
}
