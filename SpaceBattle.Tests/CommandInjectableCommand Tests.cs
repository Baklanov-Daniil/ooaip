public class Tests
{
    [Fact]
    public void Test_Inject_And_Execute()
    {
        var mockCmd = new MockCommand();
        var target = new CommandInjectableCommand();
        
        target.Inject(mockCmd);
        target.Execute();
        
        Assert.True(mockCmd.IsExecuted);
    }

    [Fact]
    public void Test_Execute_Without_Inject_Throws()
    {
        var target = new CommandInjectableCommand();
        Assert.Throws<InvalidOperationException>(() => target.Execute());
    }
}
