using App;
using Moq;

namespace SpaceBattle.Lib.Tests
{
    public class RegisterIoCDependencyMacroCommandTests : IDisposable
    {
        public RegisterIoCDependencyMacroCommandTests()
        {
            new App.Scopes.InitCommand().Execute();
            var newScope = Ioc.Resolve<System.Collections.Generic.IDictionary<string, Func<object[], object>>>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", newScope).Execute();
        }

        [Fact]
        public void Execute_ShouldResolveDependency()
        {
            var register = new RegisterIoCDependencyMacroCommand();
            var commands = new ICommand[] { Mock.Of<ICommand>() };

            register.Execute();
            
            var command = Ioc.Resolve<ICommand>("Commands.Macro", new object[] { commands });

            Assert.NotNull(command);
        }

        [Fact]
        public void Execute_ShouldResolveDependency_WithEmptyArgs()
        {
            var register = new RegisterIoCDependencyMacroCommand();
            register.Execute();

            var command = Ioc.Resolve<ICommand>("Commands.Macro");

            Assert.NotNull(command);
        }

        [Fact]
        public void Execute_ShouldThrow_WithInvalidArgumentType()
        {
            var register = new RegisterIoCDependencyMacroCommand();
            register.Execute();

            var exception = Assert.Throws<ArgumentException>(() =>
                Ioc.Resolve<ICommand>("Commands.Macro", new object[] { "invalid" }));
            
            Assert.Equal("Invalid arguments for Commands.Macro", exception.Message);
        }

        public void Dispose()
        {
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}
