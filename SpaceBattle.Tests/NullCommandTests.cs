using Xunit;

namespace SpaceBattle.Lib.Tests;

public class NullCommandTests
{
    [Fact]
    public void Execute_DoesNothingAndDoesNotThrow()
    {
        var command = new NullCommand();
        var ex = Record.Exception(() => command.Execute());
        Assert.Null(ex);
    }
}
