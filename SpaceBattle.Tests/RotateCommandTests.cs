using Moq;
namespace SpaceBattle.Lib.Tests;

public class RotateCommand_tests
{
    [Fact]
    public void AngleAndSpeedInteraction()
    {
    var ship = new Mock<IRotating>();

        ship.SetupGet(r => r.CurrentAngle).Returns(new Angle(45, 8));
        ship.SetupGet(r => r.AngleSpeed).Returns(new Angle(45, 8));

        var rotate_cmd = new RotateCommand(ship.Object);

        rotate_cmd.Execute();

        ship.VerifySet(r => r.CurrentAngle = new Angle(45, 4));
        
    }

    [Fact]
    public void AngleIsNullException()
    {
        var ship = new Mock<IRotating>();

        ship.SetupGet(r => r.CurrentAngle).Throws<InvalidOperationException>();
        ship.SetupGet(r => r.AngleSpeed).Returns(new Angle(90, 8));

        Assert.Throws<InvalidOperationException>(() => new RotateCommand(ship.Object).Execute());
    }

    [Fact]
    public void SpeedIsNullException()
    {
        var ship = new Mock<IRotating>();

        ship.SetupGet(r => r.CurrentAngle).Returns(new Angle(45, 8));
        ship.SetupGet(r => r.AngleSpeed).Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() => new RotateCommand(ship.Object).Execute());
    }

    [Fact]
    public void CantChangeAngleException()
    {
        var ship = new Mock<IRotating>();

        ship.SetupGet(r => r.CurrentAngle).Returns(new Angle(45, 8));
        ship.SetupGet(r => r.AngleSpeed).Returns(new Angle(90, 8));
        ship.SetupSet(r => r.CurrentAngle = new Angle(135, 8)).Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() => new RotateCommand(ship.Object).Execute());
    }
}
