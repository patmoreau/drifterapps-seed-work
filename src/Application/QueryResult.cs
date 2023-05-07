namespace DrifterApps.Seeds.Application;

public record QueryResult<TResult>(int Total, IEnumerable<TResult> Items);
