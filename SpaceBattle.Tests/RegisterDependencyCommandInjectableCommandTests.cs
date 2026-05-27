using Xunit;
using App;
using SpaceBattle.Lib;

namespace SpaceBattle.Tests;

public class RegisterDependencyCommandInjectableCommandTests : IDisposable
{
    public RegisterDependencyCommandInjectableCommandTests()
    {
        new App.Scopes.InitCommand().Execute();
        Ioc.Resolve<ICommand>("IoC.Scope.Current.Clear").Execute();
    }

    public void Dispose()
    {
        try
        {
            Ioc.Resolve<ICommand>("IoC.Scope.Current.Clear").Execute();
        }
        catch
        {
            // Игнорируем ошибки при очистке
        }
    }

    [Fact]
    public void AfterExecute_AllThreeResolveTypesWork_WithoutException()
    {
        // Arrange
        var registerCmd = new RegisterDependencyCommandInjectableCommand();

        // Act
        registerCmd.Execute();

        // Assert
        var asICommand = Ioc.Resolve<ICommand>("Commands.CommandInjectable");
        var asICommandInjectable = Ioc.Resolve<ICommandInjectable>("Commands.CommandInjectable");
        var asConcreteType = Ioc.Resolve<CommandInjectableCommand>("Commands.CommandInjectable");

        Assert.NotNull(asICommand);
        Assert.NotNull(asICommandInjectable);
        Assert.NotNull(asConcreteType);
    }
}
