using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace DrifterApps.Seeds.Application.Extensions;

/// <summary>
///     IQueryable extensions method for filtering and sorting from the IRequestQuery parameters.
/// </summary>
public static partial class SqlBuilderExtensions
{
    private static readonly IDictionary<string, string> Operators =
        new Dictionary<string, string>
        {
            {":eq:", "=="},
            {":ne:", "!="},
            {":gt:", ">"},
            {":ge:", ">="},
            {":lt:", "<"},
            {":le:", "<="}
        };

    /// <summary>
    ///     Filter a query from a collection of
    /// </summary>
    /// <param name="query"></param>
    /// <param name="filter"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IQueryable<T> Filter<T>(this IQueryable<T> query, IReadOnlyCollection<string> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        if (filter.Count == 0) return query;

        foreach (var f in filter)
        {
            var match = FilterRegex().Match(f);
            var property = match.Groups["property"].Value;
            var op = match.Groups["operator"].Value;
            var value = match.Groups["value"].Value;

            query = query.Where(BuildPredicate<T>(property, Operators[op], value));
        }

        return query;
    }

    public static IQueryable<T> Sort<T>(this IQueryable<T> query, IReadOnlyCollection<string> sort)
    {
        ArgumentNullException.ThrowIfNull(sort);

        if (sort.Count == 0) return query;

        var sorts = (from match in SortRegex().Matches(string.Join(";", sort))
            let asc = string.IsNullOrWhiteSpace(match.Groups["desc"].Value) ? "ASC" : "DESC"
            let field = match.Groups["field"].Value
            select $"{field} {asc}").ToList();

#pragma warning disable S3267

        var sortString = string.Join(", ", sorts);
        return query.OrderBy(sortString);
    }

    private static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName, string comparison, string value)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var left = propertyName.Split('.').Aggregate((Expression) parameter, Expression.Property);
        var body = MakeComparison(left, comparison, value);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression MakeComparison(Expression left, string comparison, string value)
    {
        switch (comparison)
        {
            case "==":
                return MakeBinary(ExpressionType.Equal, left, value);
            case "!=":
                return MakeBinary(ExpressionType.NotEqual, left, value);
            case ">":
                return MakeBinary(ExpressionType.GreaterThan, left, value);
            case ">=":
                return MakeBinary(ExpressionType.GreaterThanOrEqual, left, value);
            case "<":
                return MakeBinary(ExpressionType.LessThan, left, value);
            case "<=":
                return MakeBinary(ExpressionType.LessThanOrEqual, left, value);
            case "Contains":
            case "StartsWith":
            case "EndsWith":
                return Expression.Call(MakeString(left), comparison, Type.EmptyTypes,
                    Expression.Constant(value, typeof(string)));
            default:
                throw new NotSupportedException($"Invalid comparison operator '{comparison}'.");
        }
    }

    private static Expression MakeString(Expression source) => source.Type == typeof(string)
        ? source
        : Expression.Call(source, "ToString", Type.EmptyTypes);

    private static BinaryExpression MakeBinary(ExpressionType type, Expression left, string value)
    {
        object? typedValue = value;
        if (left.Type != typeof(string))
        {
            if (string.IsNullOrEmpty(value))
            {
                typedValue = null;
                if (Nullable.GetUnderlyingType(left.Type) == null)
                    left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
            }
            else
            {
                var valueType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
                if (valueType.IsEnum)
                    typedValue = Enum.Parse(valueType, value);
                else if (valueType == typeof(Guid))
                    typedValue = Guid.Parse(value);
                else
                    typedValue = Convert.ChangeType(value, valueType, CultureInfo.InvariantCulture);
            }
        }

        var right = Expression.Constant(typedValue, left.Type);
        return Expression.MakeBinary(type, left, right);
    }

    [GeneratedRegex("(?<property>\\w+)(?<operator>:(eq|ne|lt|le|gt|ge):)(?<value>.*)")]
    private static partial Regex FilterRegex();

    [GeneratedRegex("(?<desc>-{0,1})(?<field>\\w+)")]
    private static partial Regex SortRegex();
}
