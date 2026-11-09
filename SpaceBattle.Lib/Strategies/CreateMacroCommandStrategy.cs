using App;    

namespace SpaceBattle.Lib {
    public class CreateMacroCommandStrategy(string commandSpec)
    {
        public ICommand Resolve(object[] args)
        {
            var names = Ioc.Resolve<string[]>($"Specs.{commandSpec}", args);
            
            var commands = names.Select(name => Ioc.Resolve<ICommand>(name, args)).ToArray();
            
            return new MacroCommand(commands);
        }
    }
}
