using Bogus;

namespace DrifterApps.Seeds.Tests;

internal static class Globals
{
    private static readonly Faker s_faker = new();

    public static int RandomCollectionCount() => s_faker.Random.Int(1, 10);
}
