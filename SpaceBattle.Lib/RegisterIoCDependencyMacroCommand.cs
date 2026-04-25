using App;

namespace SpaceBattle.Lib
{
    public class RegisterIoCDependencyMacroCommand : ICommand
    {
        public void Execute()
        {
            Ioc.Resolve<App.ICommand>("IoC.Register", "Commands.Macro", (object[] args) =>
                {
                    if (args.Length == 0)
                    {
                        return new MacroCommand(Array.Empty<ICommand>());
                    }

                    if (args[0] is not ICommand[] commands)
                    {
                        throw new ArgumentException("Invalid arguments for Commands.Macro");
                    }

                    return new MacroCommand(commands);
                }).Execute();
        }
    }
}