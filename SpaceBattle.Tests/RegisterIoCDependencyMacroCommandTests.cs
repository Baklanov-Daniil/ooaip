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

        public void Dispose()
        {
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}
