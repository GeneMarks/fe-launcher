namespace Cabby.Client.Shared.Contracts.Processes
{
    public sealed record ProcessStatusUpdateDto
    {
        public required string Id { get; init; }
        public required string SessionId { get; init; }
        public required ProcessStatus Status { get; init; }
    }
}