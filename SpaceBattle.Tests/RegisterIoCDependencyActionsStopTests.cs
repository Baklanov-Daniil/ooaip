using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using App;
using App.Scopes;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Lib.Tests
{
    public class RegisterIoCDependencyActionsStopTests : IDisposable
    {
        public RegisterIoCDependencyActionsStopTests()
        {
            new InitCommand().Execute();
            var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
        }

        [Fact]
        public void ShouldRegisterStopCommandDependency()
        {
            // Регистрируем зависимость
            new RegisterIoCDependencyActionsStop().Execute();

            // Создаем корректный аргумент — словарь с очередью
            IDictionary<string, object> order = new Dictionary<string, object>
            {
                ["Queue"] = new BlockingCollection<App.ICommand>()
            };

            // Резолвим с правильным аргументом
            var cmd = Ioc.Resolve<App.ICommand>("Actions.Stop", order);

            // Проверяем, что вернулся именно StopCommand
            Assert.IsType<StopCommand>(cmd);
        }

        [Fact]
        public void Execute_StartsActionAndStopsIt()
        {
            new RegisterIoCDependencyActionsStart().Execute();
            new RegisterIoCDependencyActionsStop().Execute();

            var queue = Ioc.Resolve<BlockingCollection<App.ICommand>>("Actions.Queue.Create");
            using var executed = new ManualResetEventSlim();
            IDictionary<string, object> order = new Dictionary<string, object>
            {
                ["Queue"] = queue
            };

            var startCommand = Ioc.Resolve<App.ICommand>("Actions.Start", order);
            startCommand.Execute();
            queue.Add(new TestCommand(executed.Set));

            var stopCommand = Ioc.Resolve<App.ICommand>("Actions.Stop", order);
            stopCommand.Execute();

            Assert.True(executed.Wait(TimeSpan.FromSeconds(5)), "The started action was not executed.");
            Assert.True(((Thread)order["Thread"]).Join(TimeSpan.FromSeconds(5)), "The action thread did not stop.");
            Assert.True(queue.IsCompleted);
        }

        public void Dispose()
        {
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }

        private sealed class TestCommand : App.ICommand
        {
            private readonly Action _action;

            public TestCommand(Action action)
            {
                _action = action;
            }

            public void Execute() => _action();
        }
    }
}

