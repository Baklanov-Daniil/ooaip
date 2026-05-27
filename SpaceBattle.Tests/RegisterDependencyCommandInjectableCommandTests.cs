using SpaceBattle.Lib;
using App;


namespace SpaceBattle.Tests;

public class RegisterDependencyCommandInjectableCommandTests : IDisposable
{
    public RegisterDependencyCommandInjectableCommandTests()
    {
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
    }

    public void Dispose() => Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();


    [Fact]
    public void AfterExecute_AllThreeResolveTypesWork_WithoutException()
    {
        new RegisterDependencyCommandInjectableCommand().Execute();

        var asICommand = Ioc.Resolve<ICommand>("Commands.CommandInjectable");
        var asICommandInjectable = Ioc.Resolve<ICommandInjectable>("Commands.CommandInjectable");
        var asConcreteType = Ioc.Resolve<CommandInjectableCommand>("Commands.CommandInjectable");

        Assert.NotNull(asICommand);
        Assert.NotNull(asICommandInjectable);
        Assert.NotNull(asConcreteType);
    }
}
