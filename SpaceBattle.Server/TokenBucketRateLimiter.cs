namespace SpaceBattle.Server;

public readonly record struct RateLimitDecision(bool Allowed, TimeSpan RetryAfter);

public sealed class TokenBucketRateLimiter
{
    private sealed class Bucket(double tokens, long lastSeen)
    {
        public double Tokens { get; set; } = tokens;
        public long LastSeen { get; set; } = lastSeen;
        public long LastUsed { get; set; }
    }

    private readonly object _gate = new();
    private readonly Dictionary<string, Bucket> _buckets = new(StringComparer.Ordinal);
    private readonly double _refillPerSecond;
    private readonly double _capacity;
    private readonly int _maxEntries;
    private readonly long _ttl;
    private long _useCounter;

    public TokenBucketRateLimiter(double refillPerSecond, double capacity, int maxEntries, TimeSpan ttl, TimeProvider? timeProvider = null)
    {
        if (refillPerSecond <= 0 || capacity <= 0 || maxEntries <= 0 || ttl <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(capacity));
        _refillPerSecond = refillPerSecond;
        _capacity = capacity;
        _maxEntries = maxEntries;
        _ttl = (long)(ttl.TotalSeconds * (timeProvider ?? TimeProvider.System).TimestampFrequency);
        TimeProvider = timeProvider ?? TimeProvider.System;
    }

    public TimeProvider TimeProvider { get; }

    public RateLimitDecision TryAcquire(string username, long now)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);
        lock (_gate)
        {
            RemoveExpired(now);
            if (!_buckets.TryGetValue(username, out var bucket))
            {
                EnsureCapacity();
                bucket = new Bucket(_capacity, now);
                _buckets.Add(username, bucket);
            }

            var elapsed = Math.Max(0, now - bucket.LastSeen) / (double)TimeProvider.TimestampFrequency;
            bucket.Tokens = Math.Min(_capacity, bucket.Tokens + elapsed * _refillPerSecond);
            bucket.LastSeen = now;
            bucket.LastUsed = ++_useCounter;

            if (bucket.Tokens >= 1)
            {
                bucket.Tokens -= 1;
                return new RateLimitDecision(true, TimeSpan.Zero);
            }

            var seconds = (1 - bucket.Tokens) / _refillPerSecond;
            return new RateLimitDecision(false, TimeSpan.FromTicks((long)Math.Ceiling(seconds * TimeSpan.TicksPerSecond)));
        }
    }

    private void RemoveExpired(long now)
    {
        foreach (var pair in _buckets.Where(pair => now - pair.Value.LastSeen >= _ttl).ToArray())
            _buckets.Remove(pair.Key);
    }

    private void EnsureCapacity()
    {
        if (_buckets.Count < _maxEntries)
            return;
        var oldest = _buckets.MinBy(pair => pair.Value.LastUsed);
        if (oldest.Key is not null)
            _buckets.Remove(oldest.Key);
    }
}