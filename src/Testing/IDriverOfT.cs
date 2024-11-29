namespace DrifterApps.Seeds.Testing;

public interface IDriverOf<out TSystemUnderTest>
{
    TSystemUnderTest Build();
}
