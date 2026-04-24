using App;

namespace SpaceBattle.Lib
{
    public class RotateCommand : ICommand
    {
        public RotateCommand(IDictionary<string, object> adapter) { }
        public void Execute() { }
    }
    public class RegisterIoCDependencyRotateCommand : ICommand
    {
        public void Execute()
        {
            Ioc.Resolve<App.ICommand>("IoC.Register", "Commands.Rotate",
            (object[] args) => new RotateCommand(
                Ioc.Resolve<IDictionary<string, object>>("Adapters.IRotatingObject", args[0])));
        }
    }
}