using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace DrifterApps.Seeds.Testing;

/// <summary>
/// Abstract base class for building Faker instances.
/// </summary>
public abstract partial class FakerBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FakerBuilder"/> class.
    /// </summary>
    internal FakerBuilder() { }

    /// <summary>
    /// Creates a new Faker instance for the specified type.
    /// </summary>
    /// <typeparam name="TFaked">The type of the object to be faked.</typeparam>
    /// <returns>A new Faker instance for the specified type.</returns>
    public static Faker<TFaked> CreateFaker<TFaked>() where TFaked : class => new();

    /// <summary>
    /// Creates a new Faker instance for the specified type without initializing the object.
    /// </summary>
    /// <typeparam name="TFaked">The type of the object to be faked.</typeparam>
    /// <returns>A new Faker instance for the specified type.</returns>
    public static Faker<TFaked> CreateUninitializedFaker<TFaked>() where TFaked : class =>
            new Faker<TFaked>().CustomInstantiator(_ =>
                RuntimeHelpers.GetUninitializedObject(typeof(TFaked)) as TFaked ??
                throw new InvalidOperationException());

    /// <summary>
    /// Creates a new Faker instance for the specified type with private field binding.
    /// </summary>
    /// <typeparam name="TFaked">The type of the object to be faked.</typeparam>
    /// <returns>A new Faker instance for the specified type.</returns>
    public static Faker<TFaked> CreatePrivateFaker<TFaked>() where TFaked : class =>
            new Faker<TFaked>(binder: new BackingFieldBinder()).CustomInstantiator(_ =>
                RuntimeHelpers.GetUninitializedObject(typeof(TFaked)) as TFaked ??
                throw new InvalidOperationException());

    /// <summary>
    /// Binder for accessing private backing fields.
    /// </summary>
    private partial class BackingFieldBinder : IBinder
    {
        private const string BackingFieldSuffix = ">k__BackingField";

        /// <summary>
        /// Gets the members of the specified type.
        /// </summary>
        /// <param name="t">The type to get members from.</param>
        /// <returns>A dictionary of member names and their corresponding <see cref="MemberInfo"/>.</returns>
        public Dictionary<string, MemberInfo> GetMembers(Type t)
        {
            var availableFieldsForFakerOfT = new Dictionary<string, MemberInfo>();
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            var recursiveType = t;
            while (recursiveType is not null && recursiveType != typeof(object))
            {
                var allMembers = recursiveType.GetMembers(bindingFlags);
                var allBackingFields = allMembers.OfType<FieldInfo>()
                    .Where(FieldIsPrivate)
                    .Where(IsBackingField)
                    .ToList();
                foreach (var backingField in allBackingFields)
                {
                    var fieldName = backingField.Name[0] switch
                    {
                        '<' => ToPropertyName(backingField.Name[1..]),
                        '_' => ToPascalCase(backingField.Name[1..]),
                        _ => backingField.Name
                    };
                    availableFieldsForFakerOfT.TryAdd(fieldName, backingField);
                }

                recursiveType = recursiveType.BaseType;
            }

            return availableFieldsForFakerOfT;
        }

        /// <summary>
        /// Determines if the specified field is private.
        /// </summary>
        /// <param name="fi">The field to check.</param>
        /// <returns><c>true</c> if the field is private; otherwise, <c>false</c>.</returns>
        private static bool FieldIsPrivate(FieldInfo fi) => fi is {IsPrivate: true};

        /// <summary>
        /// Determines if the specified field is a backing field.
        /// </summary>
        /// <param name="fi">The field to check.</param>
        /// <returns><c>true</c> if the field is a backing field; otherwise, <c>false</c>.</returns>
        private static bool IsBackingField(FieldInfo fi) =>
            fi.Name.EndsWith(BackingFieldSuffix, StringComparison.InvariantCulture) || fi.Name.StartsWith('_');

        /// <summary>
        /// Converts a backing field name to a property name.
        /// </summary>
        /// <param name="input">The backing field name.</param>
        /// <returns>The property name.</returns>
        private static string ToPropertyName(string input) =>
            input.Replace(BackingFieldSuffix, "", StringComparison.InvariantCulture);

        /// <summary>
        /// Converts a string to PascalCase.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The string in PascalCase.</returns>
        private static string ToPascalCase(string input) =>
            ToPascalRegex().Replace(input, match => match.Groups[2].Value.ToUpper(CultureInfo.InvariantCulture));

        /// <summary>
        /// Gets the regex for converting to PascalCase.
        /// </summary>
        /// <returns>The regex for converting to PascalCase.</returns>
        [GeneratedRegex("(^|_)([a-z])")]
        private static partial Regex ToPascalRegex();
    }
}
