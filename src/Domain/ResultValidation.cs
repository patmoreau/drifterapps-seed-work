using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Represents the result of a validation operation.
/// </summary>
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types")]
public readonly struct ResultValidation
{
    /// <summary>
    ///     Gets the validation function.
    /// </summary>
    public Func<bool> Validation { get; private init; }

    /// <summary>
    ///     Gets the error associated with the validation result.
    /// </summary>
    public ResultError Error { get; private init; }

    /// <summary>
    ///     Creates a new instance of <see cref="ResultValidation" />.
    /// </summary>
    /// <param name="validation">The validation function.</param>
    /// <param name="error">The error associated with the validation result.</param>
    /// <returns>A new instance of <see cref="ResultValidation" />.</returns>
    public static ResultValidation Create(Func<bool> validation, ResultError error) => new()
    {
        Validation = validation, Error = error
    };
}
