using App;
using App.Scopes;

namespace SpaceBattle.Lib.Tests
{
    public class IoCRegisterGameOperationTests : IDisposable
    {
        public IoCRegisterGameOperationTests()
        {
            new InitCommand().Execute();
            var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
        }

        [Fact]
        public void Execute_RegistersGameReceiver()
        {
            var operation = new IoCRegisterGameOperation();
            operation.Execute();

            var receiver = Ioc.Resolve<ICommandReceiver>("Game.Receiver");
            Assert.NotNull(receiver);
            Assert.IsType<GameReceiver>(receiver);
        }

        [Fact]
        public void Execute_RegistersExceptionHandler()
        {
            var operation = new IoCRegisterGameOperation();
            operation.Execute();

            #pragma warning disable CS8625
            var handler = Ioc.Resolve<ICommand>("ExceptionHandler.Handle", null, null);
            #pragma warning restore CS8625
            Assert.NotNull(handler);
            Assert.IsType<NullCommand>(handler);
        }

        public void Dispose()
        {
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}
