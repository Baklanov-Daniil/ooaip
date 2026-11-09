using System.Collections.Generic;
using App;

namespace SpaceBattle.Lib;

public class IoCRegisterGameOperation : ICommand
{
    public void Execute()
    {
        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.Receiver", (object[] args) =>
        {
            return (object)new GameReceiver();
        }).Execute();

        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.CommandsQueue", (object[] args) =>
        {
            return (object)new Queue<SpaceBattle.Lib.ICommand>();
        }).Execute();

        Ioc.Resolve<App.ICommand>("IoC.Register", "Game.TimeQuant", (object[] args) =>
        {
            return (object)TimeSpan.FromMilliseconds(100);
        }).Execute();

        Ioc.Resolve<App.ICommand>("IoC.Register", "ExceptionHandler.Handle", (object[] args) =>
        {
            var cmd = args[0] as SpaceBattle.Lib.ICommand;
            var ex = args[1] as Exception;
            return (object)new NullCommand();
        }).Execute();
    }
}
