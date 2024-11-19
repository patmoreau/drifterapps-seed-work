using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Seeds.Testing;

/// <summary>
///     Provides global settings and utilities for testing.
/// </summary>
[SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
public static class Globals
{
    static Globals() => Faker.DefaultStrictMode = true;

    /// <summary>
    ///     Gets a new instance of the Faker class.
    /// </summary>
    public static Faker Fakerizer => new();

    /// <summary>
    ///     Generates a random integer between 1 and 10.
    /// </summary>
    /// <returns>A random integer between 1 and 10.</returns>
    public static int RandomCollectionCount()
    {
        var faker = new Faker();
        return faker.Random.Int(1, 10);
    }
}
