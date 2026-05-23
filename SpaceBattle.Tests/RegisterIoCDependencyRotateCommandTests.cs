using App;
using App.Scopes;
using Moq;

using SpaceBattle.Lib;

namespace SpaceBattle.Lib.Tests
{
    public class RegisterIoCDependencyRotateCommandTests : IDisposable
    {
        public RegisterIoCDependencyRotateCommandTests()
        {
            new InitCommand().Execute();
            var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
            // ИСПРАВЛЕНО: App.ICommand
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
        }

        [Fact]
        public void Execute_ShouldRegisterRotateCommandDependency()
        {
            var mockRotating = new Mock<IRotating>();
            var mockGameObject = new Mock<object>();
            
            Ioc.Resolve<App.ICommand>("IoC.Register", "Adapters.IRotating", (object[] args)
             => mockRotating.Object).Execute();

            var register = new RegisterIoCDependencyRotateCommand();
            register.Execute();

            var command = Ioc.Resolve<ICommand>("Commands.Rotate", mockGameObject.Object);
            
            Assert.NotNull(command);
        }

        public void Dispose()
        {
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}