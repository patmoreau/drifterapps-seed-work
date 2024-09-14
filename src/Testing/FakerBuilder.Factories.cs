using System.Reflection;
using System.Runtime.CompilerServices;
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
    ///     Creates a new instance of the <see cref="FakerBuilder{T}" /> class for record object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    ///     <see cref="FakerBuilder{T}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T CreateRecordBuilder<T>() where T : FakerBuilder<TFaked>, new()
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
    ///     Creates a new instance of the <see cref="FakerBuilder{T}" /> class with a backing field binder.
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

    private class BackingFieldBinder : IBinder
    {
        public Dictionary<string, MemberInfo> GetMembers(Type t)
        {
            var availableFieldsForFakerOfT = new Dictionary<string, MemberInfo>();
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            var allMembers = t.GetMembers(bindingFlags);
            var allBackingFields = allMembers.OfType<FieldInfo>()
                .Where(fi => fi is {IsPrivate: true /*, IsInitOnly: true*/})
                .Where(fi => fi.Name.EndsWith("__BackingField", StringComparison.InvariantCulture))
                .ToList();
            foreach (var backingField in allBackingFields)
            {
                var fieldName = backingField.Name[1..]
                    .Replace(">k__BackingField", "", StringComparison.InvariantCulture);
                availableFieldsForFakerOfT.TryAdd(fieldName, backingField);
            }

            return availableFieldsForFakerOfT;
        }
    }
}
