using App;
using App.Scopes;
using Moq;

using SpaceBattle.Lib;

namespace SpaceBattle.Lib.Test
{
    public class CreateMacroCommandStrategyTests : IDisposable
    {
        public CreateMacroCommandStrategyTests()
        {
            new InitCommand().Execute();
            var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
        }

        [Fact]
        public void Resolve_WithValidSpec_CreatesAndExecutesMacroCommand()
        {
            Ioc.Resolve<App.ICommand>("IoC.Register", "Specs.Test", (object[] args) => new[] { "Commands.A", "Commands.B" }
            ).Execute();
            
            var mockCmdA = Mock.Of<ICommand>();
            var mockCmdB = Mock.Of<ICommand>();
            
            Ioc.Resolve<App.ICommand>("IoC.Register", "Commands.A", (object[] args) => mockCmdA
            ).Execute();
            
            Ioc.Resolve<App.ICommand>("IoC.Register", "Commands.B", (object[] args) => mockCmdB
            ).Execute();

            var strategy = new CreateMacroCommandStrategy("Test");
            var macro = strategy.Resolve(Array.Empty<object>());
            
            macro.Execute();

            Mock.Get(mockCmdA).Verify(c => c.Execute(), Times.Once);
            Mock.Get(mockCmdB).Verify(c => c.Execute(), Times.Once);
        }

        [Fact]
        public void Resolve_WithMissingCommand_ThrowsException()
        {
            Ioc.Resolve<App.ICommand>("IoC.Register", "Specs.Test", (object[] args) => new[] { "Commands.Missing" }
            ).Execute();

            var strategy = new CreateMacroCommandStrategy("Test");

            Assert.Throws<Exception>(() => strategy.Resolve(Array.Empty<object>()));
        }

        [Fact]
        public void Resolve_InNewScope_ThrowsException()
        {
            var newScope = Ioc.Resolve<object>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", newScope).Execute();

            var strategy = new CreateMacroCommandStrategy("Test");

            Assert.Throws<Exception>(() => strategy.Resolve(Array.Empty<object>()));
        }

        public void Dispose()
        {
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}
