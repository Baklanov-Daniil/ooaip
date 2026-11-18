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
        ILogger? logger;
        try { logger = Ioc.Resolve<ILogger>("Game.Logger"); }
        catch (Exception) { logger = null; }
        var turnNumber = 0;
        var timer = Stopwatch.StartNew();

        while (timer.Elapsed < timeQuant && queue.Count > 0)
        {
            var cmd = queue.Dequeue();
            turnNumber++;
            try
            {
                cmd.Execute();
                logger?.Log(new GameTurnLog(turnNumber, cmd.GetType().Name, "succeeded", Describe(cmd)));
            }
            catch (Exception ex)
            {
                logger?.Log(new GameTurnLog(turnNumber, cmd.GetType().Name, $"failed: {ex.Message}", Describe(cmd)));
                Ioc.Resolve<ICommand>("ExceptionHandler.Handle", cmd, ex).Execute();
            }
        }
    }

    private static string Describe(ICommand command) => command is IDescribedCommand described
        ? described.Description
        : command.GetType().Name;
}
