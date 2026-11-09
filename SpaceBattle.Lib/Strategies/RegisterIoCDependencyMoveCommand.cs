using App;
namespace SpaceBattle.Lib
{
    public class RegisterIoCDependencyMoveCommand : ICommand
    {
        public void Execute()
        {
        var registerMoveCommandDependency = Ioc.Resolve<App.ICommand>(
            "IoC.Register",
            "Commands.MoveCommand",
            (object[] args) => {var movingObject = (IDictionary<string, object>)args[0];
                                var adapter = Ioc.Resolve<IMoving>("Adapters.IMovingObject", movingObject);
                                return new MoveCommand(adapter);
                                }
        );

        registerMoveCommandDependency.Execute();
        }
    }
}
