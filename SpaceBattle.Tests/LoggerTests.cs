using Xunit;

namespace SpaceBattle.Lib.Tests;

public class LoggerTests
{
    [Fact]
    public void LoggerWritesMessageToFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        var logger = new FileLogger(path);
        logger.Log("test message");

        var content = File.ReadAllText(path);
        Assert.Contains("test message", content);
    }

    [Fact]
    public void LoggerWritesMultipleMessagesToFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        var logger = new FileLogger(path);
        logger.Log("first");
        logger.Log("second");

        var content = File.ReadAllText(path);
        Assert.Contains("first", content);
        Assert.Contains("second", content);
    }
}
