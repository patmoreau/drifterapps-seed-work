namespace DrifterApps.Seeds.Testing.Drivers;

/// <summary>
///     Represents a driver for user authentication.
/// </summary>
public class UserDriver
{
    /// <summary>
    ///     Gets a value indicating whether the user is authenticated.
    /// </summary>
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AuthenticatedUser);

    /// <summary>
    ///     Gets the authenticated user ID.
    /// </summary>
    public string AuthenticatedUser { get; private set; } = string.Empty;

    /// <summary>
    ///     Authenticates the user with the specified user ID.
    /// </summary>
    /// <param name="userId">The user ID to authenticate.</param>
    public void AuthenticateUser(string userId) => AuthenticatedUser = userId;

    /// <summary>
    ///     Unauthenticates the current user.
    /// </summary>
    public void UnAuthenticate() => AuthenticatedUser = string.Empty;
}
