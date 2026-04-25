using Moq;
namespace SpaceBattle.Lib.Tests
{
    public class MoveTest
    {
        [Fact]
        public void TestObjectMove()
        {
            var ship = new Mock<IMoving>();

            ship.SetupGet(a => a.Position).Returns(new Vectors(new int[] { 12, 5 }));
            ship.SetupGet(a => a.Velocity).Returns(new Vectors(new int[] { -4, 1 }));

            var command = new MoveCommand(ship.Object);
            command.Execute();

            ship.VerifySet(a => a.Position = new Vectors(new int[] { 8, 6 }));
        }

        [Fact]
        public void TestPositionObjectCannotRead()
        {
            var ship = new Mock<IMoving>();
            ship.SetupGet(a => a.Position).Throws<InvalidOperationException>();

            var command = new MoveCommand(ship.Object);

            Assert.Throws<InvalidOperationException>(() => command.Execute());
        }

        [Fact]
        public void TestVelocityObjectCannotRead()
        {
            var ship = new Mock<IMoving>();

            ship.SetupGet(a => a.Position).Returns(new Vectors(new int[] { 12, 5 }));
            ship.SetupGet(a => a.Velocity).Throws<InvalidOperationException>();

            var command = new MoveCommand(ship.Object);

            Assert.Throws<InvalidOperationException>(() => command.Execute());
        }

        [Fact]
        public void TestImpossibleChangePositionObject()
        {
            var ship = new Mock<IMoving>();

            ship.SetupGet(a => a.Position).Returns(new Vectors(new int[] { 12, 5 }));
            ship.SetupGet(a => a.Velocity).Returns(new Vectors(new int[] { -7, 3 }));

            ship.SetupSet(a => a.Position = new Vectors(new int[] { 5, 8 })).Throws<InvalidOperationException>();

            var command = new MoveCommand(ship.Object);

            Assert.Throws<InvalidOperationException>(() => command.Execute());
        }
    }
}
