// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace DrifterApps.Seeds.Testing.Infrastructure.Authentication;

public sealed class MockAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public IEnumerable<Claim> ConfigureUserClaims(string userId) =>
        AdditionalUserClaims.TryGetValue(userId, out var additionalClaims)
            ? additionalClaims
            : Array.Empty<Claim>();

    public IDictionary<string, ICollection<Claim>> AdditionalUserClaims { get; } =
        new Dictionary<string, ICollection<Claim>>();
}
