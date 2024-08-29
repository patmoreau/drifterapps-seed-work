namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Represents the result of an operation.
/// </summary>
public class Result
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Result" /> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with the operation.</param>
    /// <exception cref="ArgumentException">Thrown when the error is invalid.</exception>
    protected Result(bool isSuccess, ResultError error)
    {
        if ((isSuccess && error != ResultError.None) || (!isSuccess && error == ResultError.None))
            throw new ArgumentException("Invalid error", nameof(error));
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
}
