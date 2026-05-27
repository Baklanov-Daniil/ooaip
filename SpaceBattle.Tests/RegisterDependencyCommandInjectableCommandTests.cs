using Xunit;
using App;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class RegisterDependencyCommandInjectableCommandTests : IDisposable
{
    public RegisterDependencyCommandInjectableCommandTests()
    {
        new App.Scopes.InitCommand().Execute();
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
    }

    public void Dispose()
    {
        Ioc.Resolve<App.ICommand>("IoC.Scope.Current.Clear").Execute();
    }

    [Fact]
    public void AfterExecute_AllThreeResolveTypesWork_WithoutException()
    {
        var registerCmd = new RegisterDependencyCommandInjectableCommand();

        registerCmd.Execute();

        var asICommand = Ioc.Resolve<ICommand>("Commands.CommandInjectable");
        var asICommandInjectable = Ioc.Resolve<ICommandInjectable>("Commands.CommandInjectable");
        var asConcreteType = Ioc.Resolve<CommandInjectableCommand>("Commands.CommandInjectable");

        Assert.NotNull(asICommand);
        Assert.NotNull(asICommandInjectable);
        Assert.NotNull(asConcreteType);
    }
}
