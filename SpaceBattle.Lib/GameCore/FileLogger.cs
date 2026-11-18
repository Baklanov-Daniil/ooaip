namespace SpaceBattle.Lib;

public sealed class FileLogger(string path) : ILogger, IDisposable
{
    private readonly StreamWriter _writer = new(path, append: false);

    public void Log(GameTurnLog entry)
    {
        var outcome = entry.Result.StartsWith("failed:", StringComparison.OrdinalIgnoreCase)
            ? $"ошибка ({entry.Result[7..].Trim()})"
            : "успешно";
        var description = string.IsNullOrWhiteSpace(entry.Description) ? entry.Initiator : entry.Description;
        _writer.WriteLine($"Ход {entry.TurnNumber}: {description} — {outcome}");
        _writer.Flush();
    }

    public void Dispose() => _writer.Dispose();
}
