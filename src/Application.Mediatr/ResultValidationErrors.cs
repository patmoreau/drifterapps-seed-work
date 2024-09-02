using DrifterApps.Seeds.Domain;
using FluentValidation.Results;

namespace DrifterApps.Seeds.Application.Mediatr;

#pragma warning disable CA1716 // Identifiers should not match keywords
/// <summary>
///     Represents a validation error with a code and description and list of <see cref="ValidationFailure" />.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Description">The error description.</param>
/// <param name="ValidationFailures">List of ValidationFailure.</param>
public record ResultValidationErrors(
    string Code,
    string Description,
    IReadOnlyList<ValidationFailure> ValidationFailures) : ResultError(Code, Description)
{
    internal static ResultValidationErrors
        ValidationErrors(Type requestType, IReadOnlyList<ValidationFailure> validationFailures) => new(
        "FluentValidation.ValidationError", $"Validation errors we found for request '{requestType.FullName}'.",
        validationFailures);
}
#pragma warning restore CA1716 // Identifiers should not match keywords
