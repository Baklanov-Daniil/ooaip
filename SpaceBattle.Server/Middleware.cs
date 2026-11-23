using System.Text.Json;

namespace SpaceBattle.Server;

public abstract class ServerMiddleware(RequestDelegate next)
{
    protected readonly RequestDelegate Next = next;
    protected static RequestState State(HttpContext context) =>
        context.Items.TryGetValue(typeof(RequestState), out var value) && value is RequestState state
            ? state
            : throw new InvalidOperationException("Request state was not initialized.");

    protected static Task Error(HttpContext context, int status, string message, int? retryAfter = null)
    {
        context.Response.StatusCode = status;
        if (retryAfter.HasValue)
            context.Response.Headers.RetryAfter = retryAfter.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        return context.Response.WriteAsJsonAsync(new { error = message });
    }
}

public sealed class RequestStateMiddleware(RequestDelegate next) : ServerMiddleware(next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Items[typeof(RequestState)] = new RequestState();
        await Next(context);
    }
}

public sealed class ParsingConcurrencyMiddleware(RequestDelegate next, ServerOptions options) : ServerMiddleware(next)
{
    private int _active;

    public async Task InvokeAsync(HttpContext context)
    {
        var count = Interlocked.Increment(ref _active);
        if (count > options.MaxConcurrentParsing)
        {
            Interlocked.Decrement(ref _active);
            context.Response.Headers.Connection = "close";
            await Error(context, StatusCodes.Status503ServiceUnavailable, "Parsing capacity exhausted", 1);
            return;
        }
        try { await Next(context); }
        finally { Interlocked.Decrement(ref _active); }
    }
}

public sealed class RequestBodyMiddleware(RequestDelegate next, ServerOptions options) : ServerMiddleware(next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentLength is long length && length > options.MaxBodyBytes)
        {
            await Error(context, StatusCodes.Status413PayloadTooLarge, "Request body is too large");
            return;
        }

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
        timeout.CancelAfter(options.ReadTimeoutMilliseconds);
        var body = new byte[options.MaxBodyBytes + 1];
        var total = 0;
        try
        {
            while (total < body.Length)
            {
                var read = await context.Request.Body.ReadAsync(body.AsMemory(total), timeout.Token);
                if (read == 0) break;
                total += read;
            }
        }
        catch (OperationCanceledException) when (!context.RequestAborted.IsCancellationRequested)
        {
            await Error(context, StatusCodes.Status408RequestTimeout, "Request body read timed out");
            return;
        }
        if (total > options.MaxBodyBytes)
        {
            await Error(context, StatusCodes.Status413PayloadTooLarge, "Request body is too large");
            return;
        }
        State(context).Body = body[..total];
        await Next(context);
    }
}

public sealed class StrictJsonMiddleware(RequestDelegate next) : ServerMiddleware(next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method) || context.Request.Path != "/command")
        {
            await Next(context);
            return;
        }
        try
        {
            using var document = JsonDocument.Parse(State(context).Body ?? []);
            var root = document.RootElement;
            RequireObject(root, "request");
            RequireProperties(root, "username", "game_id", "command");
            var command = root.GetProperty("command");
            RequireObject(command, "command");
            RequireProperties(command, "type", "args");
            var envelope = new CommandEnvelope(
                root.GetProperty("username").GetString() ?? throw new JsonException(),
                root.GetProperty("game_id").GetString() ?? throw new JsonException(),
                new CommandPayload(command.GetProperty("type").GetString() ?? throw new JsonException(), command.GetProperty("args")));
            if (string.IsNullOrWhiteSpace(envelope.Username) || string.IsNullOrWhiteSpace(envelope.GameId) || string.IsNullOrWhiteSpace(envelope.Command.Type))
                throw new JsonException();
            State(context).Envelope = envelope;
            await Next(context);
        }
        catch (JsonException)
        {
            await Error(context, StatusCodes.Status400BadRequest, "Invalid strict JSON command");
        }
    }

    private static void RequireObject(JsonElement element, string name)
    {
        if (element.ValueKind != JsonValueKind.Object) throw new JsonException(name);
    }

    private static void RequireProperties(JsonElement element, params string[] names)
    {
        var allowed = names.ToHashSet(StringComparer.Ordinal);
        if (element.EnumerateObject().Any(property => !allowed.Contains(property.Name)) || names.Any(name => !element.TryGetProperty(name, out _)))
            throw new JsonException();
    }
}

public sealed class GameValidationMiddleware(RequestDelegate next, IGameRegistry games) : ServerMiddleware(next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var envelope = State(context).Envelope!;
        if (!games.Exists(envelope.GameId))
        {
            await Error(context, StatusCodes.Status404NotFound, "Game was not found");
            return;
        }
        await Next(context);
    }
}

public sealed class ParticipantValidationMiddleware(RequestDelegate next, IGameRegistry games) : ServerMiddleware(next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var envelope = State(context).Envelope!;
        if (!games.IsParticipant(envelope.GameId, envelope.Username))
        {
            await Error(context, StatusCodes.Status403Forbidden, "Sender is not a game participant");
            return;
        }
        await Next(context);
    }
}

public sealed class RateLimitMiddleware(RequestDelegate next, TokenBucketRateLimiter limiter) : ServerMiddleware(next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var decision = limiter.TryAcquire(State(context).Envelope!.Username, TimeProvider.System.GetTimestamp());
        if (!decision.Allowed)
        {
            var retry = Math.Max(1, (int)Math.Ceiling(decision.RetryAfter.TotalSeconds));
            await Error(context, StatusCodes.Status429TooManyRequests, "Rate limit exceeded", retry);
            return;
        }
        await Next(context);
    }
}

public sealed class CommandEndpointMiddleware(RequestDelegate next, ICommandSink sink, ISequenceGenerator sequence, IRoutingTable routing) : ServerMiddleware(next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method) || context.Request.Path != "/command")
        {
            await Next(context);
            return;
        }
        var envelope = State(context).Envelope!;
        var ack = sequence.Next();
        await sink.EnqueueAsync(new QueuedCommand(ack, envelope), context.RequestAborted);
        context.Response.StatusCode = StatusCodes.Status202Accepted;
        await context.Response.WriteAsJsonAsync(new CommandResponse(ack, routing.Resolve(envelope.GameId), new { }), context.RequestAborted);
    }
}