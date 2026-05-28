using System.Collections.Concurrent;
using SpaceBattle.Lib;
using App;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyActionsStopTests : IDisposable
{
    public RegisterIoCDependencyActionsStopTests() {
        new App.Scopes.InitCommand().Execute();
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
    } 
    public void Dispose() => Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();

    [Fact]
    public void AfterExecute_ActionsStop_ResolvesWithoutException()
    {
        IDictionary<string, object> order = new Dictionary<string, object>
        {
            ["Queue"] = new BlockingCollection<App.ICommand>()
        };

        var cmd = Ioc.Resolve<App.ICommand>("Actions.Stop", order);

        Assert.NotNull(cmd);
    }

    [Fact]
    public void StopCommand_Execute_StopsRunningThread()
    {
        var queue = new BlockingCollection<App.ICommand>();
        IDictionary<string, object> order = new Dictionary<string, object>
        {
            ["Queue"] = queue
        };

        Ioc.Resolve<App.ICommand>("Actions.Start", order).Execute();
        var thread = (Thread)order["Thread"];
        Assert.True(thread.IsAlive);

        Ioc.Resolve<App.ICommand>("Actions.Stop", order).Execute();

        bool stopped = thread.Join(TimeSpan.FromSeconds(2));
        Assert.True(stopped, "Поток должен завершиться после команды Stop.");
    }

    [Fact]
    public void StopCommand_Execute_IsConstantTime()
    {
        new RegisterIoCDependencyActionsStop().Execute();

        var queue = new BlockingCollection<App.ICommand>();
        IDictionary<string, object> order = new Dictionary<string, object>
        {
            ["Queue"] = queue
        };

        var stopCmd = Ioc.Resolve<App.ICommand>("Actions.Stop", order);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        stopCmd.Execute();
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds < 100,
            $"Stop должен выполняться мгновенно, но занял {sw.ElapsedMilliseconds} мс.");
    }
}
