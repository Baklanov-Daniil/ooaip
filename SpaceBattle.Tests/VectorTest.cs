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
        Vectors vector1 = new Vectors(new int[]{1, -1, 2});
        Vectors vector2 = new Vectors(new int[]{-1, 1, -2});

        Vectors result = vector1 + vector2;

        Assert.Equal(vector1.dimension, result.dimension);
        Assert.Equal(result.coords, new int[]{0, 0, 0});
    }

    [Fact] //3
    public void ArgumentException_1()
    {
        // AAA
        Vectors vector1 = new Vectors(new int[]{1, 2, 3});
        Vectors vector2 = new Vectors(new int[]{1, 2});

        Assert.Throws<ArgumentException>(() => vector1 + vector2);
        Assert.Throws<ArgumentException>(() => vector2 + vector1);
    }

    [Fact] //4
    public void ArgumentException_2()
    {
        // AAA
        Vectors vector1 = new Vectors(new int[]{1, 2});
        Vectors vector2 = new Vectors(new int[]{1, 2, 3});

        Assert.Throws<ArgumentException>(() => vector1 + vector2);
        Assert.Throws<ArgumentException>(() => vector2 + vector1);
    }

    [Fact] //5
    public void TwoObjectsSameCoord1()
    {
        // AAA
        Vectors vector1 = new Vectors(new int[]{1, 2, 3});
        Vectors vector2 = new Vectors(new int[]{1, 2, 3});

        bool result = vector1.Equals(vector2);

        Assert.True(result);

    }

    [Fact] //6
    public void TwoObjectsSameCoord2()
    {
        // AAA
        Vectors vector1 = new Vectors(new int[]{1, 2, 3});
        Vectors vector2 = new Vectors(new int[]{1, 2, 3});

        Assert.True(vector1 == vector2);

    }

    [Fact] //7
    public void TwoObjectsDifferentCoord1()
    {
        // AAA
        Vectors vector1 = new Vectors(new int[]{1, 2, 3});
        Vectors vector2 = new Vectors(new int[]{3, 2, 1});

        bool result = vector1.Equals(vector2);

        Assert.False(result);
    }

    [Fact] //8
    public void TwoObjectsDifferentCoord2()
    {
        // AAA
        Vectors vector1 = new Vectors(new int[]{1, 2, 3});
        Vectors vector2 = new Vectors(new int[]{3, 2, 1});

        Assert.True(vector1 != vector2);
    }

    [Fact] //9
    public void IsVecrottContainsHashCode()
    {
        // AAA
        Vectors vector = new Vectors(new int[]{1,2,3});

        int hash = vector.GetHashCode();
        Assert.NotEqual(0, hash);
    }

    [Fact]
    public void VectorsConstructorThrowsArgumentExceptionWhenCoordsIsEmpty()
    {

        Assert.Throws<ArgumentException>(() => new Vectors(new int[] { }));
    }

    [Fact]
    public void VectorsEqualsReturnsFalseWhenObjectIsNotVectors()
    {
        var v = new Vectors(new int[] { 1, 2 });

        Assert.False(v.Equals("строка вместо вектора"));
    }
    [Fact]
    public void Vectors_Constructor_ThrowsException_WhenCoordsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Vectors(null!));
    }

    [Fact]
    public void VectorsOperatorEqualsReturnsTrueForSameReference()
    {
        var vector = new Vectors(new int[] { 1, 2, 3 });
        
        #pragma warning disable CS1718
        Assert.True(vector == vector);
        #pragma warning restore CS1718
    }
}