// 📁 SpaceBattle.Lib.Tests/RegisterIoCDependencyRotateCommandTests.cs
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
            Ioc.Resolve<ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
        }

        // [Fact]
        // public void Execute_ShouldRegisterRotateCommandDependency()
        // {
        //     var mockAdapter = new Mock<IDictionary<string, object>>();
        //     var mockGameObject = new Mock<object>();
            
        //     Ioc.Resolve<ICommand>(
        //         "IoC.Register",
        //         "Adapters.IRotatingObject",
        //         (object[] args) => mockAdapter.Object
        //     ).Execute();

        //     var register = new RegisterIoCDependencyRotateCommand();
        //     register.Execute();

        //     var command = Ioc.Resolve<ICommand>("Commands.Rotate", mockGameObject.Object);
            
        //     Assert.NotNull(command); 
        // }

        public void Dispose()
        {
            Ioc.Resolve<ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}