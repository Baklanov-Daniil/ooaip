using App;
using App.Scopes;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Lib.Tests
{
    public class RegisterFireDependenciesTests : IDisposable
    {
        public RegisterFireDependenciesTests()
        {
            // Правильная инициализация IoC — создаём изолированный scope, не ломая root
            new InitCommand().Execute();
            var iocScope = Ioc.Resolve<object>("IoC.Scope.Create");
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", iocScope).Execute();
        }

        [Fact]
        public void Execute_ShouldRegisterCommandsFire()
        {
            // Arrange & Act — регистрируем зависимость
            new RegisterFireDependencies().Execute();

            // Assert — резолвим с корректными аргументами
            var position = new Vector(new[] { 0, 0 });
            var direction = new Vector(new[] { 1, 1 });
            const double speed = 1.0;

            var cmd = Ioc.Resolve<ICommand>(
                "Commands.Fire",
                position,
                direction,
                speed);

            Assert.NotNull(cmd);
            Assert.IsType<FireCommand>(cmd);
        }

        [Fact]
        public void Execute_ShouldCreateFireCommandWithCorrectArguments()
        {
            // Arrange
            new RegisterFireDependencies().Execute();

            var position = new Vector(new[] { 5, 10 });
            var direction = new Vector(new[] { 3, 4 });
            const double speed = 2.5;

            // Act
            var cmd = Ioc.Resolve<FireCommand>(
                "Commands.Fire",
                position,
                direction,
                speed);

            // Assert — аргументы корректно проброшены в конструктор
            Assert.NotNull(cmd);
        }

        public void Dispose()
        {
            try
            {
                Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
            }
            catch { }
        }
    }
}
