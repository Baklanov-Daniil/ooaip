using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class RegisterDependencyCommandInjectableCommandTests_18task : IDisposable
{
    public RegisterDependencyCommandInjectableCommandTests_18task()
    {
        Ioc.Clear();
    }

    public void Dispose() => Ioc.Clear();


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
