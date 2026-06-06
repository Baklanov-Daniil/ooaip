using Xunit;
namespace SpaceBattle.Lib.Tests;

public class CollisionTests
{
    [Theory]
    [InlineData(3, 0, -2, 0, true)]   
    [InlineData(5, 5, -1, -1, false)] 
    [InlineData(0, 0, 1, 1, true)]    
    [InlineData(4, 0, 0, 5, false)]   
    public void Test_CircleCircleCollision(int x, int y, int vx, int vy, bool expected)
    {
        bool result = CollisionMath.CollideCircleCircle(x, y, vx, vy);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(4, 0, -3, 0, true)]   
    [InlineData(0, 5, 0, -2, true)]   
    [InlineData(5, 5, -1, 0, false)]  
    [InlineData(0, 0, 1, 1, true)]    
    public void Test_CircleRectangleCollision(int x, int y, int vx, int vy, bool expected)
    {
        bool result = CollisionMath.CollideCircleRectangle(x, y, vx, vy);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5, 0, -2, 0, false)]   
    [InlineData(3, 3, -5, -5, true)]  
    [InlineData(10, 10, -1, -1, false)] 
    [InlineData(0, 0, 0, 1, true)]    
    public void Test_RectangleRectangleCollision(int x, int y, int vx, int vy, bool expected)
    {
        bool result = CollisionMath.CollideRectangleRectangle(x, y, vx, vy);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Clamp()
    {
        Assert.Equal(5.0, CollisionMath.Clamp(10.0, 1.0, 5.0));  
        Assert.Equal(1.0, CollisionMath.Clamp(-5.0, 1.0, 5.0));  
        Assert.Equal(3.0, CollisionMath.Clamp(3.0, 1.0, 5.0));   
    }

    [Fact]
    public void Test_DistancePointToAabb()
    {
        double dist = CollisionMath.DistancePointToAabb(3.0, 0.0, 1.0, 2.0);
        Assert.Equal(2.0, dist, precision: 5);
    }
}

