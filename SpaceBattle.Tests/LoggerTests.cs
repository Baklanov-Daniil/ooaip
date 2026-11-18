using App;
using App.Scopes;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class LoggerTests : IDisposable
{
    public LoggerTests()
    {
        new InitCommand().Execute();
        var scope = Ioc.Resolve<object>("IoC.Scope.Create");
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", scope).Execute();
    }

    [Fact]
    public void GameLogsSuccessfulAndFailedCommands()
    {
        var log = new Mock<ILogger>();
        var queue = new Queue<SpaceBattle.Lib.ICommand>();
        var successful = new Mock<SpaceBattle.Lib.ICommand>();
        var failed = new Mock<SpaceBattle.Lib.ICommand>();
        failed.Setup(c => c.Execute()).Throws(new InvalidOperationException("failed"));
        queue.Enqueue(successful.Object);
        queue.Enqueue(failed.Object);
        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.TimeQuant", (object[] _) => (object)TimeSpan.FromSeconds(5)).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.CommandsQueue", (object[] _) => (object)queue).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Logger", (object[] _) => (object)log.Object).Execute();
        Ioc.Resolve<App.ICommand>("IoC.Register", "ExceptionHandler.Handle", (object[] _) => (object)Mock.Of<SpaceBattle.Lib.ICommand>()).Execute();

        new Game(Ioc.Resolve<object>("IoC.Scope.Current")).Execute();

        log.Verify(l => l.Log(It.Is<GameTurnLog>(entry => entry.TurnNumber == 1 && entry.Result == "succeeded")), Times.Once);
        log.Verify(l => l.Log(It.Is<GameTurnLog>(entry => entry.TurnNumber == 2 && entry.Result == "failed: failed")), Times.Once);
    }

    [Fact]
    public void FileLoggerWritesHumanReadableTurnDetails()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".log");
        using (var logger = new FileLogger(path))
            logger.Log(new GameTurnLog(1, "DemoGameCommand", "succeeded", "Подготовка игры"));

        var text = File.ReadAllText(path);
        Assert.Contains("Ход 1", text);
        Assert.Contains("Подготовка игры", text);
        Assert.Contains("успешно", text);
        File.Delete(path);
    }

    public void Dispose() => Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
}