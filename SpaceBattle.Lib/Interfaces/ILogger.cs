namespace SpaceBattle.Lib;

/// <summary>Structured outcome of one dequeued game command.</summary>
public sealed record GameTurnLog(int TurnNumber, string Initiator, string Result, string Description = "");

/// <summary>Storage-independent destination for game-turn records.</summary>
public interface ILogger
{
    void Log(GameTurnLog entry);
}

/// <summary>Default logger for interactive runs; replace through the IoC registration.</summary>
public sealed class ConsoleLogger : ILogger
{
    public void Log(GameTurnLog entry) => Console.WriteLine(
        $"Turn {entry.TurnNumber}: {entry.Initiator} — {entry.Result}");
}
