using System.Collections.Concurrent;
using System.Threading.Channels;

namespace SpaceBattle.Server;

public interface IGameRegistry
{
    bool Exists(string gameId);
    bool IsParticipant(string gameId, string username);
}

public sealed class InMemoryGameRegistry : IGameRegistry
{
    private readonly IReadOnlyDictionary<string, HashSet<string>> _games =
        new Dictionary<string, HashSet<string>>(StringComparer.Ordinal)
        {
            ["match-42"] = new HashSet<string>(new[] { "player1", "player2" }, StringComparer.Ordinal)
        };

    public bool Exists(string gameId) => _games.ContainsKey(gameId);

    public bool IsParticipant(string gameId, string username) =>
        _games.TryGetValue(gameId, out var players) && players.Contains(username);
}

public interface ICommandSink
{
    ValueTask EnqueueAsync(QueuedCommand command, CancellationToken cancellationToken);
}

public sealed class InMemoryCommandSink : ICommandSink
{
    private readonly Channel<QueuedCommand> _queue = Channel.CreateUnbounded<QueuedCommand>();

    public ValueTask EnqueueAsync(QueuedCommand command, CancellationToken cancellationToken) =>
        _queue.Writer.WriteAsync(command, cancellationToken);
}

public interface ISequenceGenerator
{
    long Next();
}

public sealed class MonotonicSequenceGenerator : ISequenceGenerator
{
    private long _sequence;

    public long Next() => Interlocked.Increment(ref _sequence);
}

public interface IRoutingTable
{
    int Resolve(string gameId);
}

public sealed class HashRoutingTable(int workerCount) : IRoutingTable
{
    public int Resolve(string gameId)
    {
        var hash = StringComparer.Ordinal.GetHashCode(gameId) & int.MaxValue;
        return hash % workerCount;
    }
}