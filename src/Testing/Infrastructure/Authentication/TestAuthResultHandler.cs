using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace DrifterApps.Seeds.Testing.Infrastructure.Authentication;

/// <summary>
///     A test implementation of <see cref="IAuthorizationMiddlewareResultHandler" /> that short-circuits
///     successful authorization responses for testing purposes.
/// </summary>
/// <remarks>
///     This handler intercepts authorization results and behaves differently than the default handler:
///     <list type="bullet">
///         <item>
///             <description>
///                 When authorization fails, it delegates to the default handler to return proper 401/403
///                 responses.
///             </description>
///         </item>
///         <item>
///             <description>
///                 When authorization succeeds, it short-circuits the pipeline and returns a 200 OK response
///                 without calling the actual endpoint, allowing tests to verify authorization logic independently.
///             </description>
///         </item>
///     </list>
/// </remarks>
public class TestAuthResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    /// <summary>
    ///     Handles the result of an authorization check during testing.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="policy">The authorization policy that was evaluated.</param>
    /// <param name="authorizeResult">The result of the authorization evaluation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="context" /> or <paramref name="authorizeResult" /> is null.
    /// </exception>
    /// <remarks>
    ///     If authorization fails, this method delegates to the default <see cref="AuthorizationMiddlewareResultHandler" />
    ///     to handle the failure response. If authorization succeeds, it returns a 200 OK response with a test message
    ///     and does not invoke the next middleware, effectively short-circuiting the request pipeline.
    /// </remarks>
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(authorizeResult);

        // If Authorization FAILED (401 or 403), let the default handler return the error
        if (!authorizeResult.Succeeded)
        {
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult).ConfigureAwait(false);
            return;
        }

        // If Authorization SUCCEEDED, short-circuit!
        // We write a specific response and DO NOT call 'next(context)'
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("Auth Success - Endpoint Not Called").ConfigureAwait(false);
    }
}
