namespace DrifterApps.Seeds.Domain;

#pragma warning disable CA1000
/// <summary>
///     Represents the result of an operation with a value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public class Result<T> : Result
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Result{T}" /> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with the operation.</param>
    /// <param name="value">The value associated with the operation.</param>
    private Result(bool isSuccess, ResultError error, T? value) : base(isSuccess, error)
    {
        if (isSuccess && value is null)
            throw new ArgumentException("Value cannot be null when the operation is successful.", nameof(value));

        Value = value;
    }

    /// <summary>
    ///     Gets the value associated with the operation.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    ///     Creates a new instance of <see cref="Result{T}" /> representing a successful operation.
    /// </summary>
    /// <param name="value">The value associated with the successful operation.</param>
    /// <returns>A new instance of <see cref="Result{T}" /> representing a successful operation.</returns>
    public static Result<T> Success(T value) => new(true, ResultError.None, value);

    /// <summary>
    ///     Creates a new instance of <see cref="Result" /> representing a failed operation.
    /// </summary>
    /// <param name="error">The error associated with the failed operation.</param>
    /// <param name="value">The value associated with the operation.</param>
    /// <returns>A new instance of <see cref="Result" /> representing a failed operation.</returns>
    public static Result<T> Failure(ResultError error, T? value = default) => new(false, error, value);
}
#pragma warning restore CA1000
