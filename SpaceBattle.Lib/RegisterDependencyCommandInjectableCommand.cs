namespace SpaceBattle.Lib;

public class RegisterDependencyCommandInjectableCommand : ICommand {
    public void Execute() {
        Ioc.Register("Commands.CommandInjectable", _ => new CommandInjectableCommand());
    }
}   
