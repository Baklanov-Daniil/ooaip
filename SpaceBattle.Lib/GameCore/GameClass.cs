using System.Diagnostics;
using App;

namespace SpaceBattle.Lib;

public class Game : ICommand
{
    private readonly object _gameScope;
    public Game(object gameScope) => _gameScope = gameScope;

    public void Execute()
    {
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Set", _gameScope).Execute();
        var queue = Ioc.Resolve<Queue<ICommand>>("Game.CommandsQueue");
        var timeQuant = Ioc.Resolve<TimeSpan>("Game.TimeQuant");
        var timer = Stopwatch.StartNew();

        while (timer.Elapsed < timeQuant && queue.Count > 0)
        {
            var cmd = queue.Dequeue();
            try
            {
                cmd.Execute();
            }
            catch (Exception ex)
            {
                Ioc.Resolve<ICommand>("ExceptionHandler.Handle", cmd, ex).Execute();
            }
        }
    }
}
