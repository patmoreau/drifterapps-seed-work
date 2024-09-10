namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Represents the result of an operation with a value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public record Result<T> : Result
{
    private readonly T? _value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Result{T}" /> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error associated with the operation.</param>
    /// <param name="value">The value associated with the operation.</param>
    private Result(bool isSuccess, ResultError error, T? value) : base(isSuccess, error)
    {
        if (isSuccess && value is null)
        {
            throw new ArgumentException("Value cannot be null when the operation is successful.", nameof(value));
        }

        _value = value;
    }

    /// <summary>
    ///     Gets the value associated with the operation.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown the result is failure.</exception>
    public T Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("Cannot access the value of a failed result.");
            }

            return _value!;
        }
    }

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
    /// <returns>A new instance of <see cref="Result" /> representing a failed operation.</returns>
    public new static Result<T> Failure(ResultError error) => new(false, error, default);
}
