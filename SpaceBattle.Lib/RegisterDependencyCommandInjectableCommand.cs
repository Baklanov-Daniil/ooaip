namespace SpaceBattle.Lib;

public class RegisterDependencyCommandInjectableCommand : ICommand {
    public void Execute() {
        IoC.Register("Commands.CommandInjectable", _ => new CommandInjectableCommand());
    }
}   
