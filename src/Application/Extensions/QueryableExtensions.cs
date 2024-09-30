using System.Linq.Dynamic.Core;
using System.Reflection;
using DrifterApps.Seeds.Domain;

namespace DrifterApps.Seeds.Application.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Query<T>(this IQueryable<T> query, QueryParams queryParams)
    {
        // Apply filters
        foreach (var filter in queryParams.Filter)
        {
            var match = QueryParams.FilterPatternRegex().Match(filter);
            if (!match.Success)
            {
                continue;
            }

            var property = match.Groups["property"].Value;
            var @operator = match.Groups["operator"].Value;
            var value = match.Groups["value"].Value;

            var comparison = @operator switch
            {
                ":eq:" => "==",
                ":ne:" => "!=",
                ":lt:" => "<",
                ":le:" => "<=",
                ":gt:" => ">",
                ":ge:" => ">=",
                _ => throw new NotSupportedException($"Invalid comparison operator '{@operator}'.")
            };

            if (IsPrimitiveType<T>(property))
            {
                property = $"{property}.Value";
            }

            query = query.Where($"{property} {comparison} @0", value);
        }

        // Apply sorting
        if (queryParams.Sort.Count != 0)
        {
            var sortString = string.Join(", ", queryParams.Sort.Select(sort =>
            {
                var match = QueryParams.SortPatternRegex().Match(sort);
                switch (match.Success)
                {
                    case true:
                    {
                        var field = match.Groups["field"].Value;
                        var direction = string.IsNullOrWhiteSpace(match.Groups["desc"].Value) ? "ASC" : "DESC";


                        if (IsPrimitiveType<T>(field))
                        {
                            field = $"{field}.Value";
                        }

                        return $"{field} {direction}";
                    }
                    default:
                        return string.Empty;
                }
            }).Where(s => !string.IsNullOrEmpty(s)));

            query = query.OrderBy(sortString);
        }

        // Apply pagination
        query = query.Skip(queryParams.Offset).Take(queryParams.Limit);

        return query;
    }

    private static bool IsPrimitiveType<T>(string property)
    {
        // Get the property type to check if it's a IPrimitiveType
        var propertyInfo = typeof(T).GetProperty(property,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo is null)
        {
            return false;
        }

        var propertyType = propertyInfo.PropertyType;
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IPrimitiveType<>))
        {
            return true;
        }

        var interfaces = propertyType.GetInterfaces();
        return Array.Exists(interfaces, @interface =>
            @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IPrimitiveType<>));
    }
}
