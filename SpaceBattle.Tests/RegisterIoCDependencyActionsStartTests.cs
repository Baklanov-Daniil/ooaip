using System.Collections.Concurrent;
using Xunit;
using App;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyActionsStartTests : IDisposable
{
    public RegisterIoCDependencyActionsStartTests()
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
        new RegisterIoCDependencyActionsStart().Execute();
        var queue = new BlockingCollection<App.ICommand>();
        IDictionary<string, object> order = new Dictionary<string, object>
        {
            ["Queue"] = queue
        };

        var wasExecuted = new AutoResetEvent(false);
        var mockCommand = new MockCommand(() => wasExecuted.Set());
        queue.Add(mockCommand);

        var startCmd = Ioc.Resolve<App.ICommand>("Actions.Start", order);
        
        startCmd.Execute();

        Assert.True(order.ContainsKey("Thread"));
        
        var thread = (Thread)order["Thread"];
        Assert.NotNull(thread);
        Assert.True(thread.IsAlive);

        bool isSignaled = wasExecuted.WaitOne(500);
        Assert.True(isSignaled);

        queue.CompleteAdding();
    }

    [Fact]
    public void Execute_RegistersActionsQueueCreate()
    {    
        var queue = Ioc.Resolve<BlockingCollection<App.ICommand>>("Actions.Queue.Create");
        
        Assert.NotNull(queue);
        Assert.IsType<BlockingCollection<App.ICommand>>(queue);
    }
    
    [Fact]
    public void Execute_RegistersBothDependencies()
    {
    
        var startCmd = Ioc.Resolve<App.ICommand>("Actions.Start", 
            new Dictionary<string, object> { ["Queue"] = new BlockingCollection<App.ICommand>() });
        Assert.NotNull(startCmd);
    
        var queue = Ioc.Resolve<BlockingCollection<App.ICommand>>("Actions.Queue.Create");
        Assert.NotNull(queue);
    }
    
    [Fact]
    public void CreatedQueue_CanAddAndTakeCommands()
    {
        var queue = Ioc.Resolve<BlockingCollection<App.ICommand>>("Actions.Queue.Create");
    
        var wasExecuted = new AutoResetEvent(false);
        var mockCommand = new MockCommand(() => wasExecuted.Set());
    
        queue.Add(mockCommand);
        queue.CompleteAdding();
    
        var commands = queue.ToList();
    
        Assert.Single(commands);
        Assert.Same(mockCommand, commands[0]);
    }
    
    private class MockCommand : App.ICommand
    {
        private readonly Action? _action;

        public MockCommand(Action? action = null)
        {
            _action = action;
        }

        public void Execute()
        {
            _action?.Invoke();
        }
    }
}
