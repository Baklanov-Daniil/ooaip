using System;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics;
using Microsoft.VisualBasic;

namespace SpaceBattle.Lib.Tests;

public class UnitTest1
{
    [Fact] //2
    public void SuccessedMoveToOrigin()
    {
        // AAA
        Vector vector1 = new Vector(new int[]{1, -1, 2});
        Vector vector2 = new Vector(new int[]{-1, 1, -2});

        Vector result = vector1 + vector2;

        Assert.Equal(vector1.dimension, result.dimension);
        Assert.Equal(result.coords, new int[]{0, 0, 0});
    }

    [Fact] //3
    public void ArgumentException_1()
    {
        // AAA
        Vector vector1 = new Vector(new int[]{1, 2, 3});
        Vector vector2 = new Vector(new int[]{1, 2});

        Assert.Throws<ArgumentException>(() => vector1 + vector2);
        Assert.Throws<ArgumentException>(() => vector2 + vector1);
    }

    [Fact] //4
    public void ArgumentException_2()
    {
        // AAA
        Vector vector1 = new Vector(new int[]{1, 2});
        Vector vector2 = new Vector(new int[]{1, 2, 3});

        Assert.Throws<ArgumentException>(() => vector1 + vector2);
        Assert.Throws<ArgumentException>(() => vector2 + vector1);
    }

    [Fact] //5
    public void TwoObjectsSameCoord1()
    {
        // AAA
        Vector vector1 = new Vector(new int[]{1, 2, 3});
        Vector vector2 = new Vector(new int[]{1, 2, 3});

        bool result = vector1.Equals(vector2);

        Assert.True(result);

    }

    [Fact] //6
    public void TwoObjectsSameCoord2()
    {
        // AAA
        Vector vector1 = new Vector(new int[]{1, 2, 3});
        Vector vector2 = new Vector(new int[]{1, 2, 3});

        Assert.True(vector1 == vector2);

    }

    [Fact] //7
    public void TwoObjectsDifferentCoord1()
    {
        // AAA
        Vector vector1 = new Vector(new int[]{1, 2, 3});
        Vector vector2 = new Vector(new int[]{3, 2, 1});

        bool result = vector1.Equals(vector2);

        Assert.False(result);
    }

    [Fact] //8
    public void TwoObjectsDifferentCoord2()
    {
        // AAA
        Vector vector1 = new Vector(new int[]{1, 2, 3});
        Vector vector2 = new Vector(new int[]{3, 2, 1});

        Assert.True(vector1 != vector2);
    }

    [Fact] //9
    public void IsVecrottContainsHashCode()
    {
        // AAA
        Vector vector = new Vector(new int[]{1,2,3});

        int hash = vector.GetHashCode();
        Assert.NotEqual(0, hash);
    }

    [Fact]
    public void VectorConstructorThrowsArgumentExceptionWhenCoordsIsEmpty()
    {

        Assert.Throws<ArgumentException>(() => new Vector(new int[] { }));
    }

    [Fact]
    public void VectorEqualsReturnsFalseWhenObjectIsNotVector()
    {
        var v = new Vector(new int[] { 1, 2 });

        Assert.False(v.Equals("строка вместо вектора"));
    }
    [Fact]
    public void Vector_Constructor_ThrowsException_WhenCoordsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Vector(null!));
    }

    [Fact]
    public void VectorOperatorEqualsReturnsTrueForSameReference()
    {
        var vector = new Vector(new int[] { 1, 2, 3 });
        
        #pragma warning disable CS1718
        Assert.True(vector == vector);
        #pragma warning restore CS1718
    }

    [Fact]
    public void VectorOperatorEquals_FirstOperandNull()
    {
        Vector? v1 = null;
        Vector? v2 = new Vector(new int[] { 1, 2, 3 });
        
        #pragma warning disable CS8604
        Assert.False(v1 == v2);
        #pragma warning restore CS8604
    }

    [Fact]
    public void VectorOperatorEquals_SecondOperandNull()
    {
        Vector? v1 = new Vector(new int[] { 1, 2, 3 });
        Vector? v2 = null;
        
        #pragma warning disable CS8604
        Assert.False(v1 == v2);
        #pragma warning restore CS8604
    }

    [Fact]
    public void VectorOperatorEquals_BothNull()
    {
        Vector? v1 = null;
        Vector? v2 = null;
        
        #pragma warning disable CS8604
        Assert.True(v1 == v2);
        #pragma warning restore CS8604
    }
    [Fact]
    public void AngleEqualsReturnsFalseForNullAndDifferentType()
    {
        var angle = new Angle(1);
        Assert.False(angle.Equals(null));
    }

    [Fact]
    public void AngleEqualsReturnsFalseForNonAngleObject()
    {
        var angle = new Angle(1);
        object notAnAngle = "это строка, а не угол";
        
        Assert.False(angle.Equals(notAnAngle));
    }

    [Fact]
    public void AngleImplicitConversionToDouble()
    {
        var angle = new Angle(2);
        double radians = angle;
        double expected = 2.0 / Angle.denominator * 2 * Math.PI;
        Assert.Equal(expected, radians, 10);
    }
}
