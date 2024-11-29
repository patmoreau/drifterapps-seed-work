using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace DrifterApps.Seeds.Testing;

public abstract partial class FakerBuilder<TFaked>
{
    public static Faker<TFaked> CreateFaker() => new();

    public static Faker<TFaked> CreateUninitializedFaker() =>
        new Faker<TFaked>().CustomInstantiator(_ =>
            RuntimeHelpers.GetUninitializedObject(typeof(TFaked)) as TFaked ??
            throw new InvalidOperationException());

    public static Faker<TFaked> CreatePrivateFaker() =>
        new Faker<TFaked>(binder: new BackingFieldBinder()).CustomInstantiator(_ =>
            RuntimeHelpers.GetUninitializedObject(typeof(TFaked)) as TFaked ??
            throw new InvalidOperationException());

    private partial class BackingFieldBinder : IBinder
    {
        private const string BackingFieldSuffix = ">k__BackingField";

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

        private static bool FieldIsPrivate(FieldInfo fi) => fi is {IsPrivate: true};

        private static bool IsBackingField(FieldInfo fi) =>
            fi.Name.EndsWith(BackingFieldSuffix, StringComparison.InvariantCulture) || fi.Name.StartsWith('_');

        private static string ToPropertyName(string input) =>
            input.Replace(BackingFieldSuffix, "", StringComparison.InvariantCulture);

        private static string ToPascalCase(string input) =>
            ToPascalRegex().Replace(input, match => match.Groups[2].Value.ToUpper(CultureInfo.InvariantCulture));

        [GeneratedRegex("(^|_)([a-z])")]
        private static partial Regex ToPascalRegex();
    }
}
