using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace DrifterApps.Seeds.Testing.Infrastructure.Authentication;

public sealed partial class JwtTokenBuilder
{
    private const string SignatureAlgorithm = SecurityAlgorithms.RsaSha256;
    private static readonly string Kid = Guid.NewGuid().ToString("N");
    private static readonly RSA Rsa = RSA.Create(2048);

    static JwtTokenBuilder()
    {
        var keyParameters = Rsa.ExportParameters(false);
        var modulus = Base64UrlEncode(keyParameters.Modulus!);
        var exponent = Base64UrlEncode(keyParameters.Exponent!);
        GetSigningKeyInfo = new SigningKeyInfo(modulus, exponent, Kid, SignatureAlgorithm);
    }

    /// <summary>
    ///     Gets the authority URL for the token issuer.
    /// </summary>
    public static string Authority => $"https://{AuthorityDomain}";

    /// <summary>
    ///     Gets the signing key information.
    /// </summary>
    public static SigningKeyInfo GetSigningKeyInfo { get; }

    /// <summary>
    ///     Gets the signing credentials for the JWT token.
    /// </summary>
    private static SigningCredentials SigningCredentials { get; } =
        new(new RsaSecurityKey(Rsa) {KeyId = Kid}, SignatureAlgorithm);

    ~JwtTokenBuilder() => Rsa.Dispose();

    private static string Base64UrlEncode(byte[] arg)
    {
        var result = Convert.ToBase64String(arg);
        result = result.Split('=')[0];
        result = result.Replace('+', '-');
        result = result.Replace('/', '_');
        return result;
    }
}
