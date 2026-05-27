using System.Collections.Concurrent;
using SpaceBattle.Lib;
using App;
using Xunit;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyActionsStartTests : IDisposable
{
    public RegisterIoCDependencyActionsStartTests() {
        new App.Scopes.InitCommand().Execute();
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
        new RegisterIoCDependencyActionsStart().Execute();
    }
    
    
    public void Dispose() => Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();

    [Fact]
    public void AfterExecute_ActionsStart_ResolvesWithoutException()
    {
        IDictionary<string, object> order = new Dictionary<string, object>
        {
            ["Queue"] = new BlockingCollection<ICommand>()
        };

        var cmd = Ioc.Resolve<App.ICommand>("Actions.Start", order);

        Assert.NotNull(cmd);
    }

    [Fact]
    public void StartCommand_Execute_StartsThread()
    {
        var queue = new BlockingCollection<ICommand>();
        IDictionary<string, object> order = new Dictionary<string, object>
        {
            ["Queue"] = queue
        };

        var startCmd = Ioc.Resolve<App.ICommand>("Actions.Start", order);
        startCmd.Execute();

        Assert.True(order.ContainsKey("Thread"));
        Assert.True(((Thread)order["Thread"]).IsAlive);

        queue.CompleteAdding();
    }
}
