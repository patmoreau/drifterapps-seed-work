using DrifterApps.Seeds.Domain;
using FluentValidation.Results;

namespace DrifterApps.Seeds.Application.Mediatr;

/// <summary>
///     Represents a validation error with a code and description and list of <see cref="ValidationFailure" />.
/// </summary>
public static class FluentValidationErrors
{
    public const string CodeValidationErrors = "FluentValidation.ValidationErrors";

    /// <summary>
    ///     Create a <see cref="ResultValidationError" /> from a <see cref="Type" /> and a list of
    ///     <see cref="ValidationFailure" />.
    /// </summary>
    /// <param name="requestType">Request <see cref="Type" /></param>
    /// <param name="validationFailures">List of <see cref="ValidationFailure" /></param>
    /// <returns>
    ///     <see cref="ResultValidationError" />
    /// </returns>
    internal static ResultValidationError ValidationErrors(Type requestType,
        IReadOnlyList<ValidationFailure> validationFailures) =>
        new(
            CodeValidationErrors,
            $"Validation errors were found for request '{requestType.FullName}'.",
            validationFailures
                .GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()));
}
