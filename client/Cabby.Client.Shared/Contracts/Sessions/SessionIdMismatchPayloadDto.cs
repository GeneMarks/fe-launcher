namespace Cabby.Client.Shared.Contracts.Sessions
{
    public sealed record SessionIdMismatchPayloadDto
    {
        public required string RequestedSessionId { get; init; }
        public required string CurrentSessionId { get; init; }
    }
}