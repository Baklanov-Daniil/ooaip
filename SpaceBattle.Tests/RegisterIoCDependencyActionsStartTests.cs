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
        try
        {
            Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        }
        catch { }
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
    public void AfterExecute_QueueCreate_ResolvesToCorrectType()
    {
        var queue = Ioc.Resolve<BlockingCollection<App.ICommand>>("Actions.Queue.Create");

        Assert.NotNull(queue);
        Assert.IsType<BlockingCollection<App.ICommand>>(queue);
    }

    [Fact]
    public void StartCommand_Execute_StartsThreadAndExecutesInternalLoop()
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
        thread.Join(100);
    }
}

public class MockCommand : App.ICommand
{
    private readonly Action _action;

    public MockCommand(Action action)
    {
        _action = action;
    }

    public void Execute()
    {
        _action();
    }
}
