using App;
using App.Scopes;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace SpaceBattle.Lib.Tests
{
    public class FireCommandTests : IDisposable
    {
        public FireCommandTests()
        {
            new InitCommand().Execute();
            var rootScope = Ioc.Resolve<object>("IoC.Scope.Current");
            var gameScope = Ioc.Resolve<object>("IoC.Scope.Create", rootScope);
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", gameScope).Execute();
        }

        [Fact]
        public void Execute_ShouldRegisterFireCommandDependency()
        {
            var startMock = new Mock<App.ICommand>();
            Ioc.Resolve<App.ICommand>("IoC.Register", "Actions.Start", (object[] args) => startMock.Object).Execute();


            var position = new Vector(new[] { 0, 0 });
            var fireDirection = new Vector(new[] { 2, 1 });
            var speed = 2.0;
            var velocity = new Vector(new[] { (int)(2 * speed), (int)(1 * speed) });

            var cmdMock = new Mock<App.ICommand>();
            var universalCommandToken = cmdMock.As<SpaceBattle.Lib.ICommand>().Object;

            var weaponMock = new Mock<IMoving>();
            weaponMock.SetupGet(a => a.Position).Returns(position);
            weaponMock.SetupGet(a => a.Velocity).Returns(velocity);
            weaponMock.SetupProperty(w => w.Position);

            var receiverMock = new Mock<ICommandReceiver>();
            var gameItems = new Dictionary<string, object>();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Weapon.Create", (object[] args) =>
            {
                return new Dictionary<string, object> { { "Id", (string)args[0] } };
            }).Execute();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Adapters.IMovingObject", (object[] args) => weaponMock.Object).Execute();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Weapon.Setup", (object[] args) =>
            {
                var weapon = (IMoving)args[0];
                weapon.Position = (Vector)args[1];
                return universalCommandToken; 
            }).Execute();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Receiver", (object[] args) => receiverMock.Object).Execute();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Item.Add", (object[] args) =>
            {
                gameItems[(string)args[0]] = args[1];
                return universalCommandToken;
            }).Execute();
            
            Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Item.Get", (object[] args) => gameItems[(string)args[0]]).Execute();
            
            Ioc.Resolve<App.ICommand>("IoC.Register", "Commands.Move", (object[] args) => universalCommandToken).Execute();

            Ioc.Resolve<App.ICommand>("IoC.Register", "Commands.Fire", (object[] args) => 
                new FireCommand(
                    (Vector)args[0], 
                    (Vector)args[1], 
                    Convert.ToDouble(args[2])
                )
            ).Execute();

            var fireCommand = Ioc.Resolve<ICommand>("Commands.Fire", position, fireDirection, speed);
            fireCommand.Execute();

            Assert.IsType<FireCommand>(fireCommand);
            startMock.Verify(s => s.Execute(), Times.Once());
        }

        [Fact]
        public void Execute_NotShouldRegisterFireCommandDependency()
        {
            Assert.ThrowsAny<Exception>(() => Ioc.Resolve<ICommand>("Commands.Fire"));
        }

        public void Dispose()
        {
            try
            {
                Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
            }
            catch { }
        }
    }
}
