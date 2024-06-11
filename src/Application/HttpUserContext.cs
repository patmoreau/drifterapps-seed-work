using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DrifterApps.Seeds.Application;

/// <inheritdoc />
internal sealed class HttpUserContext(IHttpContextAccessor httpContextAccessor) : IHttpUserContext
{
    /// <inheritdoc />
    public string IdentityObjectId => GetIdentityObjectIdFromClaims();

    private string GetIdentityObjectIdFromClaims()
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user is null) return string.Empty;

        var value = GetFirstClaimValue(user, ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(value)) value = GetFirstClaimValue(user, "sub");

        return value ?? string.Empty;
    }

    private static string? GetFirstClaimValue(ClaimsPrincipal principal, string claimType)
    {
        // Iterate over each identity in the ClaimsPrincipal
        foreach (var identity in principal.Identities)
        {
            // Get the first claim with the specified claim type
            var claim = identity.FindFirst(claimType);
            if (claim != null)
                // Return the value of the first claim found
                return claim.Value;
        }

        // No matching claim found, return null or an appropriate default value
        return null;
    }
}
