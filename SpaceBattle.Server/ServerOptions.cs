namespace SpaceBattle.Server;

public sealed class ServerOptions
{
    public int Port { get; set; } = 5080;
    public int WorkerCount { get; set; } = 8;
    public int MaxBodyBytes { get; set; } = 65536;
    public int ReadTimeoutMilliseconds { get; set; } = 2000;
    public int MaxConcurrentParsing { get; set; } = 64;
    public double RateLimitRefillPerSecond { get; set; } = 2;
    public double RateLimitBurstCapacity { get; set; } = 5;
    public int RateLimitMaxEntries { get; set; } = 10000;
    public int RateLimitEntryTtlSeconds { get; set; } = 3600;
}