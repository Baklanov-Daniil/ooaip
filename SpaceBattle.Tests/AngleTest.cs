namespace SpaceBattle.Lib.Tests;

public class AngleTests
{
    [Fact]
    public void SumOfAngles()
    {
        var a1 = new Angle(5, 8);
        var a2 = new Angle(7, 8);

        var result = a1 + a2;
        Assert.Equal("3 / 2", result.ToString());
    }

    [Fact]
    public void AnglesAreEqualReturnTrue()
    {
        var a1 = new Angle(15, 8);
        var a2 = new Angle(15, 8);

        Assert.True(a1.Equals(a2));
    }

    [Fact]
    public void SumOfAnglesOperatorReturnTrue()
    {
        var a1 = new Angle(15, 8);
        var a2 = new Angle(15, 8);

        Assert.True(a1 == a2);
    }

    [Fact]
    public void AnglesAreEqualReturnFalse()
    {
        var a1 = new Angle(1, 8);
        var a2 = new Angle(2, 8);

        Assert.False(a1.Equals(a2));
    }

    [Fact]
    public void SumOfAnglesOperatorReturnFalse()
    {
        Assert.True(new Angle(1, 8) != new Angle(2, 8));
    }

    [Fact]
    public void GetHashCodeOfAngle()
    {
        var hash = new Angle(1, 8).GetHashCode();

        Assert.NotEqual(0, hash);
    }

    [Fact]
    public void AngleEqualsReturnsFalseForNullAndDifferentType()
    {
        var angle = new Angle(1, 8);

        Assert.False(angle.Equals(null));
    }

    [Fact]
    public void AngleNotEqualOperatorWorksCorrectly()
    {
        Assert.True(new Angle(1, 8) != new Angle(2, 8));
        Assert.False(new Angle(1, 8) != new Angle(1, 8));
    }

    [Fact]
    public void AngleEqualsReturnsFalseForDifferentType()
    {
        var angle = new Angle(1, 8);
        object notAnAngle = "это строка, а не угол";

        Assert.False(angle.Equals(notAnAngle));
    }

    [Fact]
    public void AngleEqualsReturnsFalseForNonAngleObject()
    {
        var angle = new Angle(1, 8);
        object notAnAngle = "это строка, а не угол";

        Assert.False(angle.Equals(notAnAngle));
    }

    [Fact]
    public void AngleOperatorEqualsReturnsFalseForDifferentNumerators()
    {
        var a1 = new Angle(1, 8);
        var a2 = new Angle(2, 8);

        Assert.False(a1 == a2);
    }

    [Fact]
    public void AngleOperatorNotEqualsReturnsFalseForSameNumerators()
    {
        var a1 = new Angle(3, 8);
        var a2 = new Angle(11, 8);

        Assert.True(a1 != a2);
    }
}