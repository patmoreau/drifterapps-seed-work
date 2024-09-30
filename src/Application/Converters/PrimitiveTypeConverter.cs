using System.Globalization;
using System.Reflection;
using DrifterApps.Seeds.Domain;

namespace DrifterApps.Seeds.Application.Converters;

internal static class PrimitiveTypeConverter
{
    internal static object ConvertFromPrimitiveType<T>(string property, string value) =>
        IsPrimitiveType<T>(property, out var propertyType, out var primitiveType)
            ? ConvertFromPrimitiveType(value, propertyType, primitiveType)
            : value;

    private static bool IsPrimitiveType<T>(string property, out Type propertyType, out Type primitiveType)
    {
        // Get the property type to check if it's a IPrimitiveType
        var propertyInfo = typeof(T).GetProperty(property,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo is null)
        {
            propertyType = typeof(T);
            primitiveType = typeof(T);
            return false;
        }

        propertyType = propertyInfo.PropertyType;
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IPrimitiveType<>))
        {
            primitiveType = propertyType.GetGenericArguments()[0];
            return true;
        }

        var interfaces = propertyType.GetInterfaces();
        foreach (var @interface in interfaces)
        {
            if (!@interface.IsGenericType || @interface.GetGenericTypeDefinition() != typeof(IPrimitiveType<>))
            {
                continue;
            }

            primitiveType = @interface.GetGenericArguments()[0];
            return true;
        }

        primitiveType = typeof(T);
        return false;
    }

    private static object ConvertFromPrimitiveType(string value, Type propertyType, Type primitiveType)
    {
        var primitiveObject = ConvertToPrimitiveType(value, primitiveType);

        if (propertyType.IsAssignableFrom(primitiveType))
        {
            return primitiveObject;
        }

        var implicitOperator = GetImplicitOperator(propertyType, primitiveType);
        var result = implicitOperator?.Invoke(null, [primitiveObject]);
        return result ?? value;
    }

    private static object ConvertToPrimitiveType(string value, Type primitiveType) =>
        primitiveType switch
        {
            _ when primitiveType == typeof(string) => value,
            _ when primitiveType == typeof(bool) => bool.Parse(value),
            _ when primitiveType == typeof(byte) => byte.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(sbyte) => sbyte.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(char) => char.Parse(value),
            _ when primitiveType == typeof(decimal) => decimal.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(double) => double.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(float) => float.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(int) => int.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(uint) => uint.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(long) => long.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(ulong) => ulong.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(short) => short.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(ushort) => ushort.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(Guid) => Guid.Parse(value),
            _ when primitiveType == typeof(DateTime) => DateTime.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(DateTimeOffset) => DateTimeOffset.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(TimeSpan) => TimeSpan.Parse(value, CultureInfo.InvariantCulture),
            _ when primitiveType == typeof(DateOnly) => DateOnly.Parse(value, CultureInfo.InvariantCulture),
            _ => throw new NotSupportedException($"The target type '{primitiveType}' is not supported.")
        };

    private static MethodInfo? GetImplicitOperator(Type propertyType, Type primitiveType)
    {
        var typeCheck = propertyType;
        while (typeCheck is not null && typeCheck != typeof(object))
        {
            var implicitOperator = typeCheck.GetMethod("op_Implicit", [primitiveType]);
            if (implicitOperator is not null)
            {
                return implicitOperator;
            }

            typeCheck = typeCheck.BaseType;
        }

        return null;
    }
}
