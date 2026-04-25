using Xunit;

public class CommandInjectableTests
{
    [Fact]
    public void Execute_ShouldCallInjectedCommand()
    {
        var mockCommand = new MockCommand();
        
        var target = new CommandInjectableCommand();
        
        target.Inject(mockCommand);
        
        target.Execute();
        
        Assert.True(mockCommand.IsExecuted);
    }

    [Fact]
    public void Execute_Throws_If_Not_Injected()
    {
        var target = new CommandInjectableCommand();
        Assert.Throws<InvalidOperationException>(() => target.Execute());
    }
}

// заглушка для проверки факта вызова
public class MockCommand : ICommand
{
    public bool IsExecuted { get; private set; }

    public void Execute()
    {
        IsExecuted = true;
    }
}
