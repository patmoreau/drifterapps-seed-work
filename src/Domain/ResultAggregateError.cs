namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Represents an aggregate of errors with a code and description and list of <see cref="Errors" />.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Description">The error description.</param>
/// <param name="Errors">List of ResultError.</param>
public record ResultAggregateError(
    string Code,
    string Description,
    IReadOnlyCollection<ResultError> Errors) : ResultError(Code, Description)
{
    public virtual bool Equals(ResultAggregateError? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return base.Equals(other) && Errors.SequenceEqual(other.Errors);
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Errors);
}
