using DrifterApps.Seeds.Domain;

namespace DrifterApps.Seeds.Application;

public static class QueryParamsErrors
{
    public const string CodeRequestIsRequired = $"{nameof(QueryParams)}.{nameof(RequestIsRequired)}";
    public const string CodeOffsetCannotBeNegative = $"{nameof(QueryParams)}.{nameof(OffsetCannotBeNegative)}";
    public const string CodeLimitMustBePositive = $"{nameof(QueryParams)}.{nameof(CodeLimitMustBePositive)}";
    public const string CodeSortInvalidPattern = $"{nameof(QueryParams)}.{nameof(SortInvalidPattern)}";
    public const string CodeFilterInvalidPattern = $"{nameof(QueryParams)}.{nameof(FilterInvalidPattern)}";

    public static ResultError RequestIsRequired => new(CodeOffsetCannotBeNegative, "request cannot be null");
    public static ResultError OffsetCannotBeNegative => new(CodeOffsetCannotBeNegative, "offset cannot be negative");
    public static ResultError LimitMustBePositive => new(CodeLimitMustBePositive, "limit must be positive");

    public static ResultError SortInvalidPattern(string sort) =>
        new(CodeSortInvalidPattern,
            $"'{sort}' is not a valid sort pattern; should be '{QueryParams.SortPatternRegex()}'");

    public static ResultError FilterInvalidPattern(string filter) =>
        new(CodeFilterInvalidPattern,
            $"'{filter}' is not a valid filter pattern; should be '{QueryParams.FilterPatternRegex()}'");
}
