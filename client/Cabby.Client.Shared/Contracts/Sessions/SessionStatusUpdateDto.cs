namespace Cabby.Client.Shared.Contracts.Sessions
{
    public sealed record SessionStatusUpdateDto
    {
        public required string SessionId { get; init; }
        public required SessionStatus Status { get; init; }
    }
}