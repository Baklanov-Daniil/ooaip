using App;
using App.Scopes;
using Moq;
using System;
using SpaceBattle.Lib;

namespace SpaceBattle.Lib.Tests
{
    public class RegisterIoCDependencyMoveCommandTests: IDisposable
    {
        public RegisterIoCDependencyMoveCommandTests()
        {
            new InitCommand().Execute();
            var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
        }


        [Fact]
        public void Execute_ShouldRegisterMoveCommandDependency()
        {
            // arrange
            var movingObjectAdapterMock = new Mock<IMoving>();
            
            Ioc.Resolve<App.ICommand>(
                "IoC.Register", 
                "Adapters.IMovingObject", 
                (object[] args) => movingObjectAdapterMock.Object
            ).Execute();

            var movingObjectDict = new Dictionary<string, object>();
            var register = new RegisterIoCDependencyMoveCommand();

            // act
            register.Execute();

            // assert
            var resolvedCommand = Ioc.Resolve<ICommand>(
                "Commands.MoveCommand", 
                movingObjectDict
            );
    
            Assert.NotNull(resolvedCommand);
            Assert.IsType<MoveCommand>(resolvedCommand);
        }

        [Fact]
        public void Execute_ShouldntRegisterMoveCommandDependency()
        {
            var movingMock = new Mock<object>();

            Assert.ThrowsAny<Exception>(() => Ioc.Resolve<App.ICommand>("Commands.MoveCommand", movingMock));
        }
        public void Dispose()
        {
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}
