using Microsoft.AspNetCore.Authorization;

namespace DrifterApps.Seeds.Application.Authorization;

/// <summary>
///     Represents a requirement for multiple authorization policies.
/// </summary>
public sealed class MultiplePoliciesRequirement : IAuthorizationRequirement
{
    /// <summary>
    ///     Private constructor to prevent direct instantiation.
    /// </summary>
    private MultiplePoliciesRequirement()
    {
    }

    /// <summary>
    ///     Gets the collection of policies.
    /// </summary>
    public IReadOnlyCollection<string> Policies { get; private init; } = [];

    /// <summary>
    ///     Gets a value indicating whether all policies must be satisfied.
    /// </summary>
    public bool All { get; private init; }

    /// <summary>
    ///     Creates a requirement for all of the specified policies.
    /// </summary>
    /// <param name="policies">The policies that must all be satisfied.</param>
    /// <returns>A new instance of <see cref="MultiplePoliciesRequirement" />.</returns>
    /// <remarks>
    ///     Don't forget to register the <see cref="MultiplePoliciesHandler" />
    /// </remarks>
    /// <code>
    /// builder.Services.AddAuthorization(options =>
    /// {
    ///     // ... your existing policies
    ///     options.AddPolicy("CombinedAndPolicy", policy =>
    ///         policy.AddRequirements(MultiplePoliciesRequirement.ForAllOf("Policy1", "Policy2", "Policy3")));
    /// });
    /// services.AddSingleton&lt;IAuthorizationHandler, MultiplePoliciesHandler&gt;();
    /// </code>
    public static MultiplePoliciesRequirement ForAllOf(params string[] policies) =>
        new()
        {
            Policies = policies,
            All = true
        };

    /// <summary>
    ///     Creates a requirement for any of the specified policies.
    /// </summary>
    /// <param name="policies">The policies that can be satisfied.</param>
    /// <returns>A new instance of <see cref="MultiplePoliciesRequirement" />.</returns>
    /// <remarks>
    ///     Don't forget to register the <see cref="MultiplePoliciesHandler" />
    /// </remarks>
    /// <code>
    /// builder.Services.AddAuthorization(options =>
    /// {
    ///     // ... your existing policies
    ///     options.AddPolicy("CombinedAndPolicy", policy =>
    ///         policy.AddRequirements(MultiplePoliciesRequirement.ForAnyOf("Policy1", "Policy2", "Policy3")));
    /// });
    /// services.AddSingleton&lt;IAuthorizationHandler, MultiplePoliciesHandler&gt;();
    /// </code>
    public static MultiplePoliciesRequirement ForAnyOf(params string[] policies) =>
        new()
        {
            Policies = policies,
            All = false
        };
}
