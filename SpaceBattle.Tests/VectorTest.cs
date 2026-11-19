using Xunit;

namespace SpaceBattle.Lib.Tests;

public class VectorTest
{
    [Fact]
    public void SuccessedMoveToOrigin()
    {
        var vector1 = new Vector(1, -1, 2);
        var vector2 = new Vector(-1, 1, -2);

        var result = vector1 + vector2;

        Assert.Equal(vector1.Size, result.Size);
        Assert.Equal("(0, 0, 0)", result.ToString());
    }

    [Fact]
    public void ArgumentException_1()
    {
        var vector1 = new Vector(1, 2, 3);
        var vector2 = new Vector(1, 2);

        Assert.Throws<ArgumentException>(() => vector1 + vector2);
        Assert.Throws<ArgumentException>(() => vector2 + vector1);
    }

    [Fact]
    public void ArgumentException_2()
    {
        var vector1 = new Vector(1, 2);
        var vector2 = new Vector(1, 2, 3);

        Assert.Throws<ArgumentException>(() => vector1 + vector2);
        Assert.Throws<ArgumentException>(() => vector2 + vector1);
    }

    [Fact]
    public void TwoObjectsSameCoord1()
    {
        var vector1 = new Vector(1, 2, 3);
        var vector2 = new Vector(1, 2, 3);

        Assert.True(vector1.Equals(vector2));
    }

    [Fact]
    public void TwoObjectsSameCoord2()
    {
        var vector1 = new Vector(1, 2, 3);
        var vector2 = new Vector(1, 2, 3);

        Assert.True(vector1 == vector2);
    }

    [Fact]
    public void TwoObjectsDifferentCoord1()
    {
        var vector1 = new Vector(1, 2, 3);
        var vector2 = new Vector(3, 2, 1);

        Assert.False(vector1.Equals(vector2));
    }

    [Fact]
    public void TwoObjectsDifferentCoord2()
    {
        var vector1 = new Vector(1, 2, 3);
        var vector2 = new Vector(3, 2, 1);

        Assert.True(vector1 != vector2);
    }

    [Fact]
    public void IsVecrottContainsHashCode()
    {
        var vector = new Vector(1, 2, 3);

        int hash = vector.GetHashCode();
        Assert.NotEqual(0, hash);
    }

    [Fact]
    public void VectorOperatorEqualsReturnsTrueForSameReference()
    {
        var vector = new Vector(1, 2, 3);

        #pragma warning disable CS1718
        Assert.True(vector == vector);
        #pragma warning restore CS1718
    }

    [Fact]
    public void VectorOperatorEquals_FirstOperandNull()
    {
        Vector v1 = null!;
        Vector v2 = new Vector(1, 2, 3);

        #pragma warning disable CS8604
        Assert.False(v1 == v2);
        #pragma warning restore CS8604
    }

    [Fact]
    public void VectorOperatorEquals_SecondOperandNull()
    {
        Vector v1 = new Vector(1, 2, 3);
        Vector v2 = null!;

        #pragma warning disable CS8604
        Assert.False(v1 == v2);
        #pragma warning restore CS8604
    }

    [Fact]
    public void VectorOperatorEquals_BothNull()
    {
        Vector v1 = null!;
        Vector v2 = null!;

        #pragma warning disable CS8604
        Assert.True(v1 == v2);
        #pragma warning restore CS8604
    }
}