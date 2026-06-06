using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public void Dispose()
        {
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}
