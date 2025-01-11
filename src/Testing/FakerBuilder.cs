namespace DrifterApps.Seeds.Testing;

/// <summary>
///     Gets an instance of the <see cref="Faker{T}" /> class, which is used to generate fake data for testing purposes.
///     ///     Abstract base class for building instances of <typeparamref name="TFaked" /> using Faker.
/// </summary>
/// <typeparam name="TFaked">The type of class to generate instances for.</typeparam>
public abstract partial class FakerBuilder<TFaked> : FakerBuilder where TFaked : class
{
    /// <summary>
    ///     Gets an instance of the <see cref="Faker{T}" /> class,
    ///     which is used to generate fake data for testing purposes.
    /// </summary>
    protected abstract Faker<TFaked> Faker { get; }

    /// <summary>
    ///     Builds an instance of <typeparamref name="TFaked" /> using the configured rules.
    /// </summary>
    /// <returns>An instance of <typeparamref name="TFaked" />.</returns>
    public TFaked Build()
    {
        Faker.AssertConfigurationIsValid();
        return Faker.Generate();
    }

    /// <summary>
    ///     Build a collection of entities.
    /// </summary>
    /// <param name="count">
    ///     Optional number of entities to generate. If not provided, a random count will be used.
    /// </param>
    /// <returns>
    ///     A read-only collection of entities of type <typeparamref name="TFaked" />.
    /// </returns>
    public IReadOnlyCollection<TFaked> BuildCollection(int? count = null)
    {
        Faker.AssertConfigurationIsValid();
        return Faker.Generate(count ?? Globals.RandomCollectionCount());
    }
}
