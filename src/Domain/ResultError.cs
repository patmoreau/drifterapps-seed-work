namespace DrifterApps.Seeds.Domain;

#pragma warning disable CA1716 // Identifiers should not match keywords
/// <summary>
///     Represents an error with a code and description.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Description">The error description.</param>
/// <example>
///     How to implement your domain errors:
///     <code>
///     public static class DomainErrors
///     {
///         public static ResultError InvalidEmail = new("Domain.InvalidEmail", "The email is invalid.");
///         public static ResultError NotFound(Guid id) = new("Domain.NotFound", $"Domain Id '{id}' was not found.");
///     }
///     </code>
/// </example>
public record ResultError(string Code, string Description)
{
    public static readonly ResultError None = new(string.Empty, string.Empty);
}
#pragma warning restore CA1716 // Identifiers should not match keywords
