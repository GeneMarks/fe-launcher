namespace Cabby.Client.Shared.Contracts.Network
{
    public sealed record WebSocketMessage<T>
    {
        public required string Type { get; init; }
        public required T? Payload { get; init; }
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    }
}