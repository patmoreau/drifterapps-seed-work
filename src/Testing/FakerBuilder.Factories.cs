using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Bogus;

namespace DrifterApps.Seeds.Testing;

public abstract partial class FakerBuilder<TFaked>
{
    /// <summary>
    ///     Creates a new instance of the <see cref="FakerBuilder{T}" /> class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    ///     <see cref="FakerBuilder{T}" />
    /// </returns>
    public static T CreateBuilder<T>() where T : FakerBuilder<TFaked>, new()
    {
        var builder = new T
        {
            Faker = new Faker<TFaked>()
        };
        builder.ConfigureFakerRules();
        return builder;
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="FakerBuilder{T}" /> for a class without a
    ///     parameterless constructor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    ///     <see cref="FakerBuilder{T}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T CreateUninitializedBuilder<T>() where T : FakerBuilder<TFaked>, new()
    {
        var builder = new T
        {
            Faker = new Faker<TFaked>().CustomInstantiator(_ =>
                RuntimeHelpers.GetUninitializedObject(typeof(TFaked)) as TFaked ??
                throw new InvalidOperationException())
        };
        builder.ConfigureFakerRules();
        return builder;
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="FakerBuilder{T}" /> for a class with a backing field binder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    ///     <see cref="FakerBuilder{T}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T CreatePrivateBuilder<T>() where T : FakerBuilder<TFaked>, new()
    {
        var builder = new T
        {
            Faker = new Faker<TFaked>(binder: new BackingFieldBinder()).CustomInstantiator(_ =>
                RuntimeHelpers.GetUninitializedObject(typeof(TFaked)) as TFaked ??
                throw new InvalidOperationException())
        };
        builder.ConfigureFakerRules();
        return builder;
    }

    private partial class BackingFieldBinder : IBinder
    {
        private const string BackingFieldSuffix = ">k__BackingField";

        public Dictionary<string, MemberInfo> GetMembers(Type t)
        {
            var availableFieldsForFakerOfT = new Dictionary<string, MemberInfo>();
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            var allMembers = t.GetMembers(bindingFlags);
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
