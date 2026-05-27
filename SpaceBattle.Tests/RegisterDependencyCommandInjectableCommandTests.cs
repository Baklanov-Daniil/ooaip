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

[Fact]
public void Execute_RegistersActionsQueueCreate()
{
    // Arrange
    var registerCmd = new RegisterIoCDependencyActionsStart();

    // Act
    registerCmd.Execute();

    // Assert
    var queueFactory = Ioc.Resolve<Func<object[], BlockingCollection<App.ICommand>>>("Actions.Queue.Create");
    Assert.NotNull(queueFactory);

    var queue = queueFactory(new object[] { });
    Assert.NotNull(queue);
    Assert.IsType<BlockingCollection<App.ICommand>>(queue);
}

[Fact]
public void Execute_RegistersBothDependencies()
{
    // Arrange
    var registerCmd = new RegisterIoCDependencyActionsStart();

    // Act
    registerCmd.Execute();

    // Assert - проверяем что обе регистрации произошли
    var startCmd = Ioc.Resolve<App.ICommand>("Actions.Start", 
        new Dictionary<string, object> { ["Queue"] = new BlockingCollection<App.ICommand>() });
    Assert.NotNull(startCmd);

    var queueFactory = Ioc.Resolve<Func<object[], BlockingCollection<App.ICommand>>>("Actions.Queue.Create");
    Assert.NotNull(queueFactory);
}

[Fact]
public void CreatedQueue_CanAddAndTakeCommands()
{
    // Arrange
    new RegisterIoCDependencyActionsStart().Execute();
    var queueFactory = Ioc.Resolve<Func<object[], BlockingCollection<App.ICommand>>>("Actions.Queue.Create");
    var queue = queueFactory(new object[] { });

    var mockCommand = new MockCommand();

    // Act
    queue.Add(mockCommand);
    queue.CompleteAdding();

    var commands = queue.ToList();

    // Assert
    Assert.Single(commands);
    Assert.Same(mockCommand, commands[0]);
}

// Вспомогательный класс для теста
private class MockCommand : App.ICommand
{
    public bool Executed { get; private set; }
    public void Execute() => Executed = true;
}
