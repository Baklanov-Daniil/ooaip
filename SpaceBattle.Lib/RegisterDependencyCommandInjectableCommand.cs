namespace SpaceBattle.Lib;
using App;

public class RegisterDependencyCommandInjectableCommand : ICommand {
    public void Execute() {
        Ioc.Register("Commands.CommandInjectable", _ => new CommandInjectableCommand());
    }
}   
