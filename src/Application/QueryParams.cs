using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DrifterApps.Seeds.Domain;

namespace DrifterApps.Seeds.Application;

/// <summary>
///     Represents query parameters with offset, limit, sort, and filter options.
/// </summary>
public readonly partial struct QueryParams : IEquatable<QueryParams>
{
    /// <summary>
    ///     Default offset value.
    /// </summary>
    public const int DefaultOffset = 0;

    /// <summary>
    ///     Default limit value.
    /// </summary>
    public const int DefaultLimit = int.MaxValue;

    /// <summary>
    ///     Default empty sort collection.
    /// </summary>
    public static readonly IReadOnlyCollection<string> DefaultSort = ImmutableArray.Create<string>();

    /// <summary>
    ///     Default empty filter collection.
    /// </summary>
    public static readonly IReadOnlyCollection<string> DefaultFilter = ImmutableArray.Create<string>();

    /// <summary>
    ///     Gets the offset value.
    /// </summary>
    public int Offset { get; private init; }

    /// <summary>
    ///     Gets the limit value.
    /// </summary>
    public int Limit { get; private init; }

    /// <summary>
    ///     Gets the sort collection.
    /// </summary>
    public IReadOnlyCollection<string> Sort { get; private init; }

    /// <summary>
    ///     Gets the filter collection.
    /// </summary>
    public IReadOnlyCollection<string> Filter { get; private init; }

    /// <summary>
    ///     Gets an empty instance of <see cref="QueryParams" />.
    /// </summary>
    public static QueryParams Empty => new()
    {
        Offset = DefaultOffset,
        Limit = DefaultLimit,
        Sort = DefaultSort,
        Filter = DefaultFilter
    };

    /// <summary>
    ///     Determines whether the specified <see cref="QueryParams" /> is equal to the current <see cref="QueryParams" />.
    /// </summary>
    /// <param name="other">The <see cref="QueryParams" /> to compare with the current <see cref="QueryParams" />.</param>
    /// <returns>
    ///     true if the specified <see cref="QueryParams" /> is equal to the current <see cref="QueryParams" />;
    ///     otherwise, false.
    /// </returns>
    public bool Equals(QueryParams other) => Offset == other.Offset &&
                                             Limit == other.Limit &&
                                             Sort.SequenceEqual(other.Sort) &&
                                             Filter.SequenceEqual(other.Filter);

    /// <summary>
    ///     Determines whether the specified object is equal to the current <see cref="QueryParams" />.
    /// </summary>
    /// <param name="obj">The object to compare with the current <see cref="QueryParams" />.</param>
    /// <returns>true if the specified object is equal to the current <see cref="QueryParams" />; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is QueryParams other && Equals(other);

    /// <summary>
    ///     Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current <see cref="QueryParams" />.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Offset);
        hash.Add(Limit);
        foreach (var item in Sort)
        {
            hash.Add(item);
        }

        foreach (var item in Filter)
        {
            hash.Add(item);
        }

        return hash.ToHashCode();
    }

    /// <summary>
    ///     Determines whether two specified instances of <see cref="QueryParams" /> are equal.
    /// </summary>
    /// <param name="left">The first <see cref="QueryParams" /> to compare.</param>
    /// <param name="right">The second <see cref="QueryParams" /> to compare.</param>
    /// <returns>true if the two <see cref="QueryParams" /> instances are equal; otherwise, false.</returns>
    public static bool operator ==(QueryParams left, QueryParams right) => left.Equals(right);

    /// <summary>
    ///     Determines whether two specified instances of <see cref="QueryParams" /> are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="QueryParams" /> to compare.</param>
    /// <param name="right">The second <see cref="QueryParams" /> to compare.</param>
    /// <returns>true if the two <see cref="QueryParams" /> instances are not equal; otherwise, false.</returns>
    public static bool operator !=(QueryParams left, QueryParams right) => !(left == right);

    /// <summary>
    ///     Creates a new instance of <see cref="QueryParams" /> from the specified <see cref="IRequestQuery" />.
    /// </summary>
    /// <param name="requestQuery">The request query to create the <see cref="QueryParams" /> from.</param>
    /// <returns>A <see cref="Result{QueryParams}" /> representing the result of the creation.</returns>
    public static Result<QueryParams> Create(IRequestQuery requestQuery) =>
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        requestQuery is null
            ? Result<QueryParams>.Failure(QueryParamsErrors.RequestIsRequired)
            : Create(requestQuery.Offset, requestQuery.Limit, requestQuery.Sort, requestQuery.Filter);

    /// <summary>
    ///     Creates a new instance of <see cref="QueryParams" /> with the specified parameters.
    /// </summary>
    /// <param name="offset">The offset value.</param>
    /// <param name="limit">The limit value.</param>
    /// <param name="sort">The sort collection.</param>
    /// <param name="filter">The filter collection.</param>
    /// <returns>A <see cref="Result{QueryParams}" /> representing the result of the creation.</returns>
    public static Result<QueryParams> Create(int offset, int limit, IReadOnlyCollection<string> sort,
        IReadOnlyCollection<string> filter)
    {
        var result = Result.Validate(
            OffsetValidation(offset),
            LimitValidation(limit),
            SortValidation(sort),
            FilterValidation(filter));
        return result.IsFailure
            ? Result<QueryParams>.Failure(result.Error)
            : Result<QueryParams>.Success(new QueryParams
            {
                Offset = offset,
                Limit = limit,
                Sort = sort,
                Filter = filter
            });
    }

    /// <summary>
    ///     Validates the offset value.
    /// </summary>
    /// <param name="value">The offset value to validate.</param>
    /// <returns>A <see cref="ResultValidation" /> representing the result of the validation.</returns>
    private static ResultValidation OffsetValidation(int value) =>
        ResultValidation.Create(() => value >= 0, QueryParamsErrors.OffsetCannotBeNegative);

    /// <summary>
    ///     Validates the limit value.
    /// </summary>
    /// <param name="value">The limit value to validate.</param>
    /// <returns>A <see cref="ResultValidation" /> representing the result of the validation.</returns>
    private static ResultValidation LimitValidation(int value) =>
        ResultValidation.Create(() => value > 0, QueryParamsErrors.LimitMustBePositive);

    /// <summary>
    ///     Validates the sort collection.
    /// </summary>
    /// <param name="sorts">The sort collection to validate.</param>
    /// <returns>A <see cref="ResultValidation" /> representing the result of the validation.</returns>
    private static ResultValidation SortValidation(IEnumerable<string> sorts)
    {
        var errors = new StringBuilder();
        return ResultValidation.Create(() =>
        {
            var isValid = true;
            foreach (var sort in sorts)
            {
                var match = SortPatternRegex().Match(sort);
                if (match.Success)
                {
                    continue;
                }

                isValid = false;
                errors.Append(CultureInfo.InvariantCulture, $"'{sort}' is invalid");
                errors.AppendLine();
            }

            return isValid;
        }, QueryParamsErrors.SortInvalidPattern(errors.ToString()));
    }

    /// <summary>
    ///     Validates the filter collection.
    /// </summary>
    /// <param name="filters">The filter collection to validate.</param>
    /// <returns>A <see cref="ResultValidation" /> representing the result of the validation.</returns>
    private static ResultValidation FilterValidation(IEnumerable<string> filters)
    {
        var errors = new StringBuilder();
        return ResultValidation.Create(() =>
        {
            var isValid = true;
            foreach (var filter in filters)
            {
                var match = FilterPatternRegex().Match(filter);
                if (match.Success)
                {
                    continue;
                }

                isValid = false;
                errors.Append(CultureInfo.InvariantCulture, $"'{filter}' is invalid");
                errors.AppendLine();
            }

            return isValid;
        }, QueryParamsErrors.FilterInvalidPattern(errors.ToString()));
    }

    /// <summary>
    ///     Gets the regex pattern for filter validation.
    /// </summary>
    /// <returns>The regex pattern for filter validation.</returns>
    [GeneratedRegex("^(?<property>\\w+)(?<operator>:(eq|ne|lt|le|gt|ge):)(?<value>.+)$")]
    internal static partial Regex FilterPatternRegex();

    /// <summary>
    ///     Gets the regex pattern for sort validation.
    /// </summary>
    /// <returns>The regex pattern for sort validation.</returns>
    [GeneratedRegex("^(?<desc>-{0,1})(?<field>\\w+)$")]
    internal static partial Regex SortPatternRegex();
}
