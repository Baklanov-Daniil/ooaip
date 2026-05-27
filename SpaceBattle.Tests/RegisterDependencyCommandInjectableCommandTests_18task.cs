using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class RegisterDependencyCommandInjectableCommandTests_18task : IDisposable
{
    public RegisterDependencyCommandInjectableCommandTests_18task()
    {
        IoC.Clear();
    }

    public void Dispose() => IoC.Clear();


    [Fact]
    public void AfterExecute_AllThreeResolveTypesWork_WithoutException()
    {
        new RegisterDependencyCommandInjectableCommand().Execute();

        var asICommand = IoC.Resolve<ICommand>("Commands.CommandInjectable");
        var asICommandInjectable = IoC.Resolve<ICommandInjectable>("Commands.CommandInjectable");
        var asConcreteType = IoC.Resolve<CommandInjectableCommand>("Commands.CommandInjectable");

        Assert.NotNull(asICommand);
        Assert.NotNull(asICommandInjectable);
        Assert.NotNull(asConcreteType);
    }
}
