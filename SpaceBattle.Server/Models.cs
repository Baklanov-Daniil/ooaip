using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceBattle.Server;

public sealed record CommandEnvelope(string Username, string GameId, CommandPayload Command);

public sealed record CommandPayload(string Type, JsonElement Args);

public sealed record QueuedCommand(long Sequence, CommandEnvelope Envelope);

public sealed record CommandResponse(
    long Ack,
    [property: JsonPropertyName("routing_hash")] int RoutingHash,
    [property: JsonPropertyName("game_snapshot")] object GameSnapshot);

public sealed class RequestState
{
    public CommandEnvelope? Envelope { get; set; }
    public byte[]? Body { get; set; }
}