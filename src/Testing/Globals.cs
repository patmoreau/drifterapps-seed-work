using Bogus;

namespace DrifterApps.Seeds.Testing;

public static class Globals
{
#pragma warning disable CA1810
    static Globals()
#pragma warning restore CA1810
    {
        Faker.DefaultStrictMode = true;
        Fakerizer = new Faker();
    }

    public static Faker Fakerizer { get; }

    public static int RandomCollectionCount() => Fakerizer.Random.Int(1, 10);
}
