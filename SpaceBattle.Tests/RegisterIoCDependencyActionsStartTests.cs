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
}
