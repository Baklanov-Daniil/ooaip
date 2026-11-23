using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.TestHost;
using SpaceBattle.Server;

namespace SpaceBattle.Lib.Tests;

public sealed class ServerIntegrationTests : IClassFixture<ServerFactory>
{
    private readonly HttpClient _client;

    public ServerIntegrationTests(ServerFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task ValidCommandsReturnMonotonicAcks()
    {
        var first = await _client.PostAsJsonAsync("/command", ValidCommand("player1"));
        var second = await _client.PostAsJsonAsync("/command", ValidCommand("player1"));
        var firstAck = await first.Content.ReadFromJsonAsync<AckResponse>();
        var secondAck = await second.Content.ReadFromJsonAsync<AckResponse>();
        Assert.Equal(HttpStatusCode.Accepted, first.StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, second.StatusCode);
        Assert.True(secondAck!.Ack > firstAck!.Ack);
    }

    [Theory]
    [InlineData("{\"username\":\"player1\",\"game_id\":\"match-42\",\"command\":{\"type\":\"Start\",\"args\":{}},\"extra\":1}", 400)]
    [InlineData("{\"username\":\"player1\",\"game_id\":\"missing\",\"command\":{\"type\":\"Start\",\"args\":{}}}", 404)]
    [InlineData("{\"username\":\"outsider\",\"game_id\":\"match-42\",\"command\":{\"type\":\"Start\",\"args\":{}}}", 403)]
    public async Task ValidationReturnsExpectedStatus(string json, int status)
    {
        var response = await _client.PostAsync("/command", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(status, (int)response.StatusCode);
    }

    [Fact]
    public async Task OversizedBodyReturns413()
    {
        var response = await _client.PostAsync("/command", new StringContent(new string('x', 70000)));
        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
    }

    [Fact]
    public async Task RateLimitIsPerPlayerAndReturns429()
    {
        using var factory = new RateLimitFactory();
        using var client = factory.CreateClient();
        for (var i = 0; i < 2; i++)
            Assert.Equal(HttpStatusCode.Accepted, (await client.PostAsJsonAsync("/command", ValidCommand("player1"))).StatusCode);
        var limited = await client.PostAsJsonAsync("/command", ValidCommand("player1"));
        var otherPlayer = await client.PostAsJsonAsync("/command", ValidCommand("player2"));
        Assert.Equal(HttpStatusCode.TooManyRequests, limited.StatusCode);
        Assert.True(limited.Headers.RetryAfter?.Delta > TimeSpan.Zero);
        Assert.Equal(HttpStatusCode.Accepted, otherPlayer.StatusCode);
    }

    [Fact]
    public async Task ParsingCapacityReturns503AndClosesConnection()
    {
        using var factory = new ParsingCapacityFactory();
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/command", ValidCommand("player1"));
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("1", response.Headers.RetryAfter?.ToString());
        Assert.Equal("close", response.Headers.Connection.ToString());
    }

    private static object ValidCommand(string username) => new
    {
        username,
        game_id = "match-42",
        command = new { type = "Start", args = new { } }
    };

    private sealed record AckResponse(long Ack, int RoutingHash, object GameSnapshot);
}

public sealed class ServerFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Server:Port"] = "5081",
            ["Server:RateLimitBurstCapacity"] = "100",
            ["Server:RateLimitRefillPerSecond"] = "100"
        }));
        return base.CreateHost(builder);
    }
}

public sealed class RateLimitFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<TokenBucketRateLimiter>();
            services.AddSingleton(new TokenBucketRateLimiter(0.1, 2, 100, TimeSpan.FromMinutes(1)));
        });
        base.ConfigureWebHost(builder);
    }
}

public sealed class ParsingCapacityFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ServerOptions>();
            services.AddSingleton(new ServerOptions { MaxConcurrentParsing = 0 });
        });
        base.ConfigureWebHost(builder);
    }
}