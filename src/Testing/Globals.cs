using Bogus;

namespace DrifterApps.Seeds.Testing;

public static class Globals
{
#pragma warning disable CA1810
    static Globals()
#pragma warning restore CA1810
    {
        Faker.DefaultStrictMode = true;
        // Fakerizer = new Faker();
    }

    public static Faker Fakerizer => new Faker();

    public static int RandomCollectionCount()
    {
        var faker = new Faker();
        return faker.Random.Int(1, 10);
    }
}
