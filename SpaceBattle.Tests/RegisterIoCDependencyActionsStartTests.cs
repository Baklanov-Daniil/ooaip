using System.Collections.Concurrent;
using Xunit;
using App;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyActionsStartTests_19task : IDisposable
{
    public RegisterIoCDependencyActionsStartTests_19task()
    {
        new App.Scopes.InitCommand().Execute();
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
    }

    public void Dispose()
    {
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
    }

    [Fact]
    public void AfterExecute_ActionsStart_ResolvesWithoutException()
    {
        new RegisterIoCDependencyActionsStart().Execute();

        IDictionary<string, object> order = new Dictionary<string, object>
        {
            ["Queue"] = new BlockingCollection<App.ICommand>()
        };

        var cmd = Ioc.Resolve<App.ICommand>("Actions.Start", order);

        Assert.NotNull(cmd);
    }

    [Fact]
    public void StartCommand_Execute_StartsThread()
    {
        var queue = new BlockingCollection<App.ICommand>();
        IDictionary<string, object> order = new Dictionary<string, object>
        {
            ["Queue"] = queue
        };

        var startCmd = Ioc.Resolve<App.ICommand>("Actions.Start", order);
        
        startCmd.Execute();

        Assert.True(order.ContainsKey("Thread"));
        
        var thread = (Thread)order["Thread"];
        Assert.NotNull(thread);
        Assert.True(thread.IsAlive);

        queue.CompleteAdding();
    }

    [Fact]
    public void Execute_RegistersActionsQueueCreate()
    {
        // Arrange
        var registerCmd = new RegisterIoCDependencyActionsStart();
    
        // Act
        registerCmd.Execute();
    
        // Assert
        var queueFactory = Ioc.Resolve<Func<object[], BlockingCollection<App.ICommand>>>("Actions.Queue.Create");
        Assert.NotNull(queueFactory);
    
        var queue = queueFactory(new object[] { });
        Assert.NotNull(queue);
        Assert.IsType<BlockingCollection<App.ICommand>>(queue);
    }
    
    [Fact]
    public void Execute_RegistersBothDependencies()
    {
        // Arrange
        var registerCmd = new RegisterIoCDependencyActionsStart();
    
        // Act
        registerCmd.Execute();
    
        // Assert - проверяем что обе регистрации произошли
        var startCmd = Ioc.Resolve<App.ICommand>("Actions.Start", 
            new Dictionary<string, object> { ["Queue"] = new BlockingCollection<App.ICommand>() });
        Assert.NotNull(startCmd);
    
        var queueFactory = Ioc.Resolve<Func<object[], BlockingCollection<App.ICommand>>>("Actions.Queue.Create");
        Assert.NotNull(queueFactory);
    }
    
    [Fact]
    public void CreatedQueue_CanAddAndTakeCommands()
    {
        // Arrange
        new RegisterIoCDependencyActionsStart().Execute();
        var queueFactory = Ioc.Resolve<Func<object[], BlockingCollection<App.ICommand>>>("Actions.Queue.Create");
        var queue = queueFactory(new object[] { });
    
        var mockCommand = new MockCommand();
    
        // Act
        queue.Add(mockCommand);
        queue.CompleteAdding();
    
        var commands = queue.ToList();
    
        // Assert
        Assert.Single(commands);
        Assert.Same(mockCommand, commands[0]);
    }
    
    // Вспомогательный класс для теста
    private class MockCommand : App.ICommand
    {
        public bool Executed { get; private set; }
        public void Execute() => Executed = true;
    }
}
