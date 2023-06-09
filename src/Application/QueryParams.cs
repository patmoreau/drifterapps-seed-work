using System.Collections.Immutable;

namespace DrifterApps.Seeds.Application;

/// <summary>
/// Class for supporting query params to be handled in requests
/// </summary>
public class QueryParams
{
    public const string FilterPattern = @"(?<property>\w+)(?<operator>:(eq|ne|lt|le|gt|ge):)(?<value>.*)";
    public const string SortPattern = @"(?<desc>-{0,1})(?<field>\w+)";

    /// <summary>
    /// Default offset value
    /// </summary>
    public const int DefaultOffset = 0;
    /// <summary>
    /// Default limit value
    /// </summary>
    public const int DefaultLimit = int.MaxValue;
    /// <summary>
    /// Default empty sort
    /// </summary>
    public static readonly IReadOnlyCollection<string> DefaultSort = ImmutableArray.Create<string>();
    /// <summary>
    /// Default empty filter
    /// </summary>
    public static readonly IReadOnlyCollection<string> DefaultFilter = ImmutableArray.Create<string>();

    private QueryParams(int offset, int limit, IEnumerable<string> sort, IEnumerable<string> filter)
    {
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), @"offset cannot be negative");
        }

        if (limit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), @"limit must be positive");
        }

        if (sort is null)
        {
            throw new ArgumentNullException(nameof(sort));
        }

        if (filter is null)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        Offset = offset;
        Limit = limit;
        Sort = ImmutableArray.CreateRange(sort);
        Filter = ImmutableArray.CreateRange(filter);
    }

    public int Offset { get; }
    public int Limit { get; }
    public IReadOnlyCollection<string> Sort { get; }
    public IReadOnlyCollection<string> Filter { get; }

    public static QueryParams Empty => new(DefaultOffset, DefaultLimit, DefaultSort, DefaultFilter);

    public static QueryParams Create(IRequestQuery requestQuery)
    {
        if (requestQuery == null)
        {
            throw new ArgumentNullException(nameof(requestQuery));
        }

        return new QueryParams(requestQuery.Offset, requestQuery.Limit, requestQuery.Sort, requestQuery.Filter);
    }
}
