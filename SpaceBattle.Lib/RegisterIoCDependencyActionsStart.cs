using System.Collections.Concurrent;
using App;

namespace SpaceBattle.Lib;

public class RegisterIoCDependencyActionsStart : App.ICommand
{
    public void Execute()
    {
        try
        {
            Ioc.Resolve<object>("Actions.Start");
        }
        catch
        {
            Ioc.Resolve<App.ICommand>(
                "IoC.Register", 
                "Actions.Start", 
                (object[] args) => new StartCommand((IDictionary<string, object>)args[0])
            ).Execute();
        }

        try
        {
            Ioc.Resolve<object>("Actions.Queue.Create");
        }
        catch
        {
            Ioc.Resolve<App.ICommand>(
                "IoC.Register", 
                "Actions.Queue.Create", 
                (object[] args) => new BlockingCollection<App.ICommand>()
            ).Execute();
        }
    }
}
