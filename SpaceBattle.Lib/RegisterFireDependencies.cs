using App;
namespace SpaceBattle.Lib
{
    public class RegisterFireDependencies : ICommand
    {
        public void Execute()
        {
            Ioc.Resolve<App.ICommand>("IoC.Register", "Commands.Fire", (object[] args) =>
            {
                return new FireCommand((Vectors)args[0], (Vectors)args[1], (double)args[2]);
            }).Execute();
        }
    }
}
