using System.Globalization;
using Bogus;

namespace DrifterApps.Seeds.Domain.Tests;

[UnitTest]
public class StronglyTypedIdTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void GivenNew_WhenInvoked_ThenReturnNewId()
    {
        // arrange

        // act
        var result = MyId.New;

        // assert
        result.Should().NotBeNull();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public void GivenEmpty_WhenInvoked_ThenReturnEmptyId()
    {
        // arrange

        // act
        var result = MyId.Empty;

        // assert
        result.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public void GivenCompareTo_WhenOtherIsNull_ThenReturn1()
    {
        // arrange
        var id = MyId.New;

        // act
        var result = id.CompareTo(null);

        // assert
        result.Should().Be(1);
    }

    [Fact]
    public void GivenCompareTo_WhenOtherIsLower_ThenReturn1()
    {
        // arrange
        var id = MyId.New;

        // act
        var result = id.CompareTo(MyId.Empty);

        // assert
        result.Should().Be(1);
    }

    [Fact]
    public void GivenCompareTo_WhenOtherIsHigher_ThenReturnMinus1()
    {
        // arrange
        var id = MyId.Empty;

        // act
        var result = id.CompareTo(MyId.New);

        // assert
        result.Should().Be(-1);
    }

    [Fact]
    public void GivenCompareTo_WhenOtherIsEqual_ThenReturn0()
    {
        // arrange
        var id = MyId.New;

        // act
        var result = id.CompareTo(MyId.Create(id.Value));

        // assert
        result.Should().Be(0);
    }

    [Theory]
    [ClassData(typeof(EqualityComparerEqualsData))]
    public void GivenEqualityComparerEquals_WhenInvoked_ThenReturnExpected(MyId? x, MyId? y, bool expected)
    {
        // arrange
        var id = MyId.New;

        // act
        var result = id.Equals(x, y);

        // assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GivenGetHashCode_WhenObjIsNull_ThenThrowArgumentNullException()
    {
        // arrange
        var id = MyId.New;

        // act
        Action act = () => _ = id.GetHashCode(null!);

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GivenGetHashCode_WhenObjIsValid_ThenReturnHashCodeOfValue()
    {
        // arrange
        var id = MyId.New;

        // act
        var result = id.GetHashCode(id);

        // assert
        result.Should().Be(id.Value.GetHashCode());
    }

    [Fact]
    public void GivenCreate_WhenInvoked_ThenReturnStronglyTypedId()
    {
        // arrange
        var id = _faker.Random.Guid();

        // act
        var result = MyId.Create(id);

        // assert
        result.Should().NotBeNull();
        result.Value.Should().Be(id);
    }

    [Theory]
    [ClassData(typeof(EqualsData))]
    public void GivenEquals_WhenBothNull_ThenReturnTrue(MyId? other, bool expected)
    {
        // arrange
        var id = MyId.Empty;

        // act
        var result = id.Equals(other);

        // assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GivenGetHashCode_WhenInvoked_ThenReturnHashCodeOfValue()
    {
        // arrange
        var guid = _faker.Random.Guid();
        var id = MyId.Create(guid);

        // act
        var result = id.GetHashCode();

        // assert
        result.Should().Be(guid.GetHashCode());
    }

    [Theory]
    [ClassData(typeof(OperatorEqualData))]
    public void GivenOperatorEqual_WhenInvoked_ThenReturnExpected(MyId? a, MyId? b, bool expected)
    {
        // arrange

        // act
        var result = a == b;

        // assert
        result.Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(OperatorNotEqualData))]
    public void GivenOperatorNotEqual_WhenInvoked_ThenReturnExpected(MyId? a, MyId? b, bool expected)
    {
        // arrange

        // act
        var result = a != b;

        // assert
        result.Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(OperatorGreaterThanData))]
    public void GivenOperatorGreaterThan_WhenInvoked_ThenReturnExpected(MyId? a, MyId? b, bool expected)
    {
        // arrange

        // act
        var result = a > b;

        // assert
        result.Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(OperatorLowerThanData))]
    public void GivenOperatorLowerThan_WhenInvoked_ThenReturnExpected(MyId? a, MyId? b, bool expected)
    {
        // arrange

        // act
        var result = a < b;

        // assert
        result.Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(OperatorGreaterEqualThanData))]
    public void GivenOperatorGreaterEqualThan_WhenInvoked_ThenReturnExpected(MyId? a, MyId? b, bool expected)
    {
        // arrange

        // act
        var result = a >= b;

        // assert
        result.Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(OperatorLowerEqualThanData))]
    public void GivenOperatorLowerEqualThan_WhenInvoked_ThenReturnExpected(MyId? a, MyId? b, bool expected)
    {
        // arrange

        // act
        var result = a <= b;

        // assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GivenOperatorGuid_WhenCast_ThenReturnGuid()
    {
        // arrange
        var id = MyId.New;

        // act
        var result = (Guid) id;

        // assert
        result.Should().Be(id.Value);
    }

    [Fact]
    public void GivenOperatorStronglyTypedId_WhenCast_ThenReturnId()
    {
        // arrange
        var id = _faker.Random.Guid();

        // act
        var result = (MyId) id;

        // assert
        result.Should().BeOfType(typeof(MyId));
        result.Value.Should().Be(id);
    }

    [Fact]
    public void GivenParse_WhenStringIsNotGuid_ThenReturnEmpty()
    {
        // arrange
        var id = _faker.Random.String();

        // act
        var result = MyId.Parse(id, CultureInfo.InvariantCulture);

        // assert
        result.Should().Be(MyId.Empty);
    }

    [Fact]
    public void GivenParse_WhenStringIsGuid_ThenReturnNewId()
    {
        // arrange
        var guid = _faker.Random.Guid();

        // act
        var result = MyId.Parse(guid.ToString(), CultureInfo.InvariantCulture);

        // assert
        result.Should().NotBe(MyId.Empty);
        result.Value.Should().Be(guid);
    }

    [Fact]
    public void GivenTryParse_WhenStringIsNull_ThenReturnFalseEmpty()
    {
        // arrange

        // act
        var result = MyId.TryParse(null, CultureInfo.InvariantCulture, out var id);

        // assert
        result.Should().BeFalse();
        id.Should().Be(MyId.Empty);
    }

    [Fact]
    public void GivenTryParse_WhenStringIsNotGuid_ThenReturnFalseEmpty()
    {
        // arrange
        var guid = _faker.Random.String();

        // act
        var result = MyId.TryParse(guid, CultureInfo.InvariantCulture, out var id);

        // assert
        result.Should().BeFalse();
        id.Should().Be(MyId.Empty);
    }

    [Fact]
    public void GivenTryParse_WhenStringIsGuid_ThenReturnTrueId()
    {
        // arrange
        var guid = _faker.Random.Guid();

        // act
        var result = MyId.TryParse(guid.ToString(), CultureInfo.InvariantCulture, out var id);

        // assert
        result.Should().BeTrue();
        id.Should().NotBeNull();
        id!.Value.Should().Be(guid);
    }

    [Fact]
    public void GivenCompareTo_WhenStringIsGuid_ThenReturnEqual()
    {
        // arrange
        var guid = _faker.Random.Guid();
        var myId = MyId.Create(guid);

        // act

        var result = myId == guid.ToString();

        // assert
        result.Should().BeTrue();
    }

    public record MyId : StronglyTypedId<MyId>;

    public class EqualityComparerEqualsData : TheoryData<MyId?, MyId?, bool>
    {
        public EqualityComparerEqualsData()
        {
            Add(null, null, true);
            Add(null, MyId.New, false);
            Add(MyId.New, null, false);
            Add(MyId.New, MyId.New, false);
            Add(MyId.Empty, MyId.Empty, true);
        }
    }

    public class EqualsData : TheoryData<MyId?, bool>
    {
        public EqualsData()
        {
            Add(null, false);
            Add(MyId.New, false);
            Add(MyId.Empty, true);
        }
    }

    public class OperatorEqualData : TheoryData<MyId?, MyId?, bool>
    {
        public OperatorEqualData()
        {
            Add(null, null, true);
            Add(null, MyId.New, false);
            Add(MyId.New, null, false);
            Add(MyId.Empty, MyId.Empty, true);
            Add(MyId.New, MyId.Empty, false);
        }
    }

    public class OperatorNotEqualData : TheoryData<MyId?, MyId?, bool>
    {
        public OperatorNotEqualData()
        {
            Add(null, null, false);
            Add(null, MyId.New, true);
            Add(MyId.New, null, true);
            Add(MyId.Empty, MyId.Empty, false);
            Add(MyId.New, MyId.Empty, true);
        }
    }

    public class OperatorGreaterThanData : TheoryData<MyId?, MyId?, bool>
    {
        public OperatorGreaterThanData()
        {
            Add(null, null, false);
            Add(null, MyId.New, false);
            Add(MyId.New, null, true);
            Add(MyId.Empty, MyId.Empty, false);
            Add(MyId.New, MyId.Empty, true);
            Add(MyId.Empty, MyId.New, false);
        }
    }

    public class OperatorLowerThanData : TheoryData<MyId?, MyId?, bool>
    {
        public OperatorLowerThanData()
        {
            Add(null, null, false);
            Add(null, MyId.New, true);
            Add(MyId.New, null, false);
            Add(MyId.Empty, MyId.Empty, false);
            Add(MyId.New, MyId.Empty, false);
            Add(MyId.Empty, MyId.New, true);
        }
    }

    public class OperatorGreaterEqualThanData : TheoryData<MyId?, MyId?, bool>
    {
        public OperatorGreaterEqualThanData()
        {
            Add(null, null, true);
            Add(null, MyId.New, false);
            Add(MyId.New, null, true);
            Add(MyId.Empty, MyId.Empty, true);
            Add(MyId.New, MyId.Empty, true);
            Add(MyId.Empty, MyId.New, false);
        }
    }

    public class OperatorLowerEqualThanData : TheoryData<MyId?, MyId?, bool>
    {
        public OperatorLowerEqualThanData()
        {
            Add(null, null, true);
            Add(null, MyId.New, true);
            Add(MyId.New, null, false);
            Add(MyId.Empty, MyId.Empty, true);
            Add(MyId.New, MyId.Empty, false);
            Add(MyId.Empty, MyId.New, true);
        }
    }
}
