namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Represents a validation error with a code and description and list of <see cref="Errors" />.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Description">The error description.</param>
/// <param name="Errors">Dictionary of validation failures by property name.</param>
public record ResultValidationError(
    string Code,
    string Description,
    IReadOnlyDictionary<string, string[]> Errors) : ResultError(Code, Description);
