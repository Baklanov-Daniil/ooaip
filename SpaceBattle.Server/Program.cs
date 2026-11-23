using SpaceBattle.Server;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ServerOptions>(builder.Configuration.GetSection("Server"));
var options = builder.Configuration.GetSection("Server").Get<ServerOptions>() ?? new ServerOptions();
builder.WebHost.UseUrls($"http://0.0.0.0:{options.Port}");
builder.Services.AddSingleton(options);
builder.Services.AddSingleton<IGameRegistry, InMemoryGameRegistry>();
builder.Services.AddSingleton<ICommandSink, InMemoryCommandSink>();
builder.Services.AddSingleton<ISequenceGenerator, MonotonicSequenceGenerator>();
builder.Services.AddSingleton<IRoutingTable>(_ => new HashRoutingTable(options.WorkerCount));
builder.Services.AddSingleton(_ => new TokenBucketRateLimiter(options.RateLimitRefillPerSecond, options.RateLimitBurstCapacity, options.RateLimitMaxEntries, TimeSpan.FromSeconds(options.RateLimitEntryTtlSeconds)));

var app = builder.Build();
app.UseMiddleware<RequestStateMiddleware>();
app.UseMiddleware<ParsingConcurrencyMiddleware>();
app.UseMiddleware<RequestBodyMiddleware>();
app.UseMiddleware<StrictJsonMiddleware>();
app.UseMiddleware<GameValidationMiddleware>();
app.UseMiddleware<ParticipantValidationMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();
app.UseMiddleware<CommandEndpointMiddleware>();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();

public partial class Program;