using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DrifterApps.Seeds.Application;

/// <inheritdoc />
internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    /// <inheritdoc />
    public Guid Id => GetUserId();

    private Guid GetUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user is null) return Guid.Empty;

        var value = GetFirstClaimValue(user, ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(value)) value = GetFirstClaimValue(user, "sub");

        return string.IsNullOrWhiteSpace(value) ? Guid.Empty : Guid.Parse(value);
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
