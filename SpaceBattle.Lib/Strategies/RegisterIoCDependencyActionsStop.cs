using System.Collections.Concurrent;
using App;
namespace SpaceBattle.Lib;

public class StopCommand : App.ICommand
{
    private readonly IDictionary<string, object> _order;

    public StopCommand(IDictionary<string, object> order)
    {
        _order = order;
    }

    public void Execute()
    {
        var queue = (BlockingCollection<App.ICommand>)_order["Queue"];
        queue.Add(new CompleteQueueCommand(queue));
    }
}

public class CompleteQueueCommand : App.ICommand
{
    private readonly BlockingCollection<App.ICommand> _queue;

    public CompleteQueueCommand(BlockingCollection<App.ICommand> queue)
    {
        _queue = queue;
    }

    public void Execute()
    {
        _queue.CompleteAdding();
    }
}

public class RegisterIoCDependencyActionsStop : App.ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>(
            "IoC.Register", 
            "Actions.Stop", 
            (object[] args) => new StopCommand((IDictionary<string, object>)args[0])
        ).Execute();
    }
}
