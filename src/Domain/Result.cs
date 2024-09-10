namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Represents the result of an operation.
/// </summary>
public record Result
{
    public const string CodeValidateErrors = $"{nameof(Result)}.{nameof(Validate)}";

    /// <summary>
    ///     Initializes a new instance of the <see cref="Result" /> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with the operation.</param>
    /// <exception cref="ArgumentException">Thrown when the error is invalid.</exception>
    protected Result(bool isSuccess, ResultError error)
    {
        if ((isSuccess && error != ResultError.None) || (!isSuccess && error == ResultError.None))
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    ///     Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    ///     Gets a value indicating whether the operation was a failure.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    ///     Gets the error associated with the operation.
    /// </summary>
    public ResultError Error { get; }

    /// <summary>
    ///     Creates a new instance of <see cref="Result" /> representing a successful operation.
    /// </summary>
    /// <returns>A new instance of <see cref="Result" /> representing a successful operation.</returns>
    public static Result Success() => new(true, ResultError.None);

    /// <summary>
    ///     Creates a new instance of <see cref="Result" /> representing a failed operation.
    /// </summary>
    /// <param name="error">The error associated with the failed operation.</param>
    /// <returns>A new instance of <see cref="Result" /> representing a failed operation.</returns>
    public static Result Failure(ResultError error) => new(false, error);

    /// <summary>
    ///     Validates the result of a <see cref="ResultValidation" />.
    /// </summary>
    /// <param name="firstValidator">The struct used for validation.</param>
    /// <param name="additionalValidators">Additional validators.</param>
    /// <returns>A new instance of <see cref="Result" /> representing the validation result.</returns>
    public static Result Validate(ResultValidation firstValidator, params ResultValidation[] additionalValidators)
    {
        List<ResultError> errors = [];
        if (!firstValidator.Validation())
        {
            errors.Add(firstValidator.Error);
        }

        errors.AddRange(from validator in additionalValidators ?? []
            where !validator.Validation()
            select validator.Error);

        return errors is {Count: 0}
            ? Success()
            : Failure(new ResultAggregateError(CodeValidateErrors,
#pragma warning disable S3358
                $"{errors.Count} validation{(errors.Count > 1 ? "s" : string.Empty)} failed", [.. errors]));
#pragma warning restore S3358
    }
}
