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
        public void Execute_RegistersCommandsQueue()
        {
            var operation = new IoCRegisterGameOperation();
            operation.Execute();

            var queue = Ioc.Resolve<Queue<SpaceBattle.Lib.ICommand>>("Game.CommandsQueue");
            Assert.NotNull(queue);
            Assert.Empty(queue);
            Assert.IsType<Queue<SpaceBattle.Lib.ICommand>>(queue);
        }

        [Fact]
        public void Execute_RegistersTimeQuant()
        {
            var operation = new IoCRegisterGameOperation();
            operation.Execute();

            var timeQuant = Ioc.Resolve<TimeSpan>("Game.TimeQuant");
            Assert.Equal(TimeSpan.FromMilliseconds(100), timeQuant);
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

        [Fact]
        public void Execute_RegistersAllDependencies()
        {
            var operation = new IoCRegisterGameOperation();
            operation.Execute();

            Assert.NotNull(Ioc.Resolve<ICommandReceiver>("Game.Receiver"));
            Assert.NotNull(Ioc.Resolve<Queue<SpaceBattle.Lib.ICommand>>("Game.CommandsQueue"));
            #pragma warning disable CS8625
            Assert.NotNull(Ioc.Resolve<ICommand>("ExceptionHandler.Handle", null, null));
            #pragma warning restore CS8625
        }

        public void Dispose()
        {
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}
