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
public void Angle_Equals_ReturnsFalseForNullAndDifferentType()
{
    var angle = new Angle(1);

    Assert.False(angle.Equals(null));

    Assert.False(angle.Equals("не угол"));
}

[Fact]
public void Angle_NotEqualOperator_WorksCorrectly()
{
    var a1 = new Angle(1);
    var a2 = new Angle(2);
    var a3 = new Angle(1);

    Assert.True(a1 != a2);

    Assert.False(a1 != a3);
}
}