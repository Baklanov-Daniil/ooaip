using System.Collections.Concurrent;
using SpaceBattle.Lib;
using App;
using Xunit;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyActionsStartTests_19task : IDisposable
{
    public RegisterIoCDependencyActionsStartTests_19task() => Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
    public void Dispose() => Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();

    [Fact]
    public void AfterExecute_ActionsStart_ResolvesWithoutException()
    {
        new RegisterIoCDependencyActionsStart().Execute();

        IDictionary<string, object> order = new Dictionary<string, object>
        {
            ["Queue"] = new BlockingCollection<ICommand>()
        };

        var cmd = Ioc.Resolve<ICommand>("Actions.Start", order);

        Assert.NotNull(cmd);
    }

    [Fact]
    public void StartCommand_Execute_StartsThread()
    {
        new RegisterIoCDependencyActionsStart().Execute();

        var queue = new BlockingCollection<ICommand>();
        IDictionary<string, object> order = new Dictionary<string, object>
        {
            ["Queue"] = queue
        };

        var startCmd = Ioc.Resolve<ICommand>("Actions.Start", order);
        startCmd.Execute();

        Assert.True(order.ContainsKey("Thread"));
        Assert.True(((Thread)order["Thread"]).IsAlive);

        queue.CompleteAdding();
    }
}
