using Microsoft.AspNetCore.Http;

namespace DrifterApps.Seeds.Application;

/// <summary>
///     Interface to extract user context information fro the <see cref="IHttpContextAccessor" />.
/// </summary>
public interface IUserContext
{
    /// <summary>
    ///     Id extracted from user claims
    /// </summary>
    Guid Id { get; }
}
