using System.Reflection.Metadata;


namespace SpaceBattle.Lib.Tests;

public class AngleTests
{
    [Fact]
    public void SumOfAngles()
    {
        //AAA
        var a1 = new Angle(5);
        var a2 = new Angle(7);

        var result = a1 + a2;
        int expected = 4;

        Assert.Equal(result.numenator, expected);

    }

    [Fact]
    public void AnglesAreEqualReturnTrue()
    {
        var a1 = new Angle(15);
        var a2 = new Angle(23);

        var result = a1.Equals(a2);

        Assert.True(result);
    }

    [Fact]
    public void SumOfAnglesOperatorReturnTrue()
    {
        var a1 = new Angle(15);
        var a2 = new Angle(23);

        Assert.True(a1 == a2);
    }

    [Fact]
    public void AnglesAreEqualReturnFalse()
    {
        var a1 = new Angle(1);
        var a2 = new Angle(2);

        var result = a1.Equals(a2);

        Assert.False(result);
    }

    [Fact]
    public void SumOfAnglesOperatorReturnFalse()
    {
        var a1 = new Angle(1);
        var a2 = new Angle(2);

        Assert.True(a1 != a2);
    }

    [Fact]
    public void GetHashCodeOfAngle()
    {
        var a1 = new Angle(1);

        var hash = a1.GetHashCode();

        Assert.NotEqual(0, hash);
    }

    [Fact]
    public void AngleEqualsReturnsFalseForNullAndDifferentType()
    {
        var angle = new Angle(1);

        Assert.False(angle.Equals(null));
    }

    [Fact]
    public void AngleNotEqualOperatorWorksCorrectly()
    {
        var a1 = new Angle(1);
        var a2 = new Angle(2);
        var a3 = new Angle(1);

        Assert.True(a1 != a2);

        Assert.False(a1 != a3);
    }

    [Fact]
    public void AngleImplicitConversionToDouble()
    {
        var angle = new Angle(2);
        
        double radians = angle;
        
        double expected = 2.0 / Angle.denominator * 2 * Math.PI;
        
        Assert.Equal(expected, radians, 10);
    }

    [Fact]
    public void AngleEqualsReturnsFalseForDifferentType()
    {
        var angle = new Angle(1);
        object notAnAngle = "это строка, а не угол";
        
        Assert.False(angle.Equals(notAnAngle));
    }

    [Fact]
    public void AngleEqualsReturnsFalseForNonAngleObject()
    {
        var angle = new Angle(1);
        object notAnAngle = "это строка, а не угол";
        
        Assert.False(angle.Equals(notAnAngle));
    }

    [Fact]
    public void AngleOperatorEqualsReturnsFalseForDifferentNumerators()
    {
        var a1 = new Angle(1);
        var a2 = new Angle(2);
        
        Assert.False(a1 == a2);
    }
}
