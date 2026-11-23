using SpaceBattle.Server;

namespace SpaceBattle.Lib.Tests;

public class TokenBucketRateLimiterTests
{
    private static readonly long Second = TimeProvider.System.TimestampFrequency;

    [Fact]
    public void AllowsBurstThenRejects()
    {
        var limiter = new TokenBucketRateLimiter(1, 2, 10, TimeSpan.FromMinutes(1));
        Assert.True(limiter.TryAcquire("one", 0).Allowed);
        Assert.True(limiter.TryAcquire("one", 0).Allowed);
        var decision = limiter.TryAcquire("one", 0);
        Assert.False(decision.Allowed);
        Assert.Equal(TimeSpan.FromSeconds(1), decision.RetryAfter);
    }

    [Fact]
    public void RefillsAndKeepsPlayersIndependent()
    {
        var limiter = new TokenBucketRateLimiter(2, 1, 10, TimeSpan.FromMinutes(1));
        Assert.True(limiter.TryAcquire("one", 0).Allowed);
        Assert.False(limiter.TryAcquire("one", 0).Allowed);
        Assert.True(limiter.TryAcquire("one", Second / 2).Allowed);
        Assert.True(limiter.TryAcquire("two", 0).Allowed);
    }

    [Fact]
    public void ExpiredEntriesAreEvicted()
    {
        var limiter = new TokenBucketRateLimiter(1, 1, 1, TimeSpan.FromSeconds(10));
        Assert.True(limiter.TryAcquire("one", 0).Allowed);
        Assert.True(limiter.TryAcquire("two", Second * 11).Allowed);
    }
}