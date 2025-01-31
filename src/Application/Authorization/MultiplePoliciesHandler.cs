using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace DrifterApps.Seeds.Application.Authorization;

/// <summary>
///     Handles multiple authorization policies for a given requirement.
/// </summary>
/// <param name="serviceProvider"><see cref="IServiceProvider"/></param>
/// <remarks>
///     This handler checks multiple policies and determines if the requirement is met based on the specified conditions.
/// </remarks>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
public class MultiplePoliciesHandler(IServiceProvider serviceProvider)
    : AuthorizationHandler<MultiplePoliciesRequirement>
{
    /// <summary>
    ///     Handles the authorization requirement by checking multiple policies.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The requirement containing the policies to check.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MultiplePoliciesRequirement requirement)
    {
        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
        foreach (var policyName in requirement.Policies)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(context.User, policyName)
                .ConfigureAwait(false);

            switch (requirement.All)
            {
                case true when !authorizationResult.Succeeded:
                    return;
                case false when authorizationResult.Succeeded:
                    context.Succeed(requirement);
                    return;
            }
        }

        if (requirement.All)
        {
            context.Succeed(requirement);
        }
    }
}
