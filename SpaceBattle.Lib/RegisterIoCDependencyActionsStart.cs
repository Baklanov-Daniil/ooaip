using System.Collections.Concurrent;
using App;

namespace SpaceBattle.Lib;

public class StartCommand : App.ICommand
{
    private readonly IDictionary<string, object> _order;

    public StartCommand(IDictionary<string, object> order)
    {
        _order = order;
    }

    public void Execute()
    {
        var queue = (BlockingCollection<App.ICommand>)_order["Queue"];

        var thread = new Thread(() =>
        {
            foreach (var cmd in queue.GetConsumingEnumerable())
            {
                cmd.Execute();
            }
        })
        { IsBackground = true };

        _order["Thread"] = thread;
        thread.Start();
    }
}

public class RegisterIoCDependencyActionsStart : App.ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
            "IoC.Register", 
            "Actions.Start", 
            (object[] args) => new StartCommand((IDictionary<string, object>)args[0])
        ).Execute();


        Ioc.Resolve<App.ICommand>(
            "IoC.Register", 
            "Actions.Queue.Create", 
            (object[] args) => new BlockingCollection<App.ICommand>()
        ).Execute();
    }
}
