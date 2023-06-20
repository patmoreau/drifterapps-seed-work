namespace DrifterApps.Seeds.Application;

/// <summary>
///     Query results from <see cref="IRequestQuery" />
/// </summary>
/// <param name="Total">Total of items resulting from the query</param>
/// <param name="Items">List of items resulting from the query</param>
/// <typeparam name="TResult">Type of items</typeparam>
public record QueryResult<TResult>(int Total, IEnumerable<TResult> Items);
