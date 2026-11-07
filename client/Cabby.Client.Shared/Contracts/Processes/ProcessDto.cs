namespace Cabby.Client.Shared.Contracts.Processes
{
    public sealed record ProcessDto
    {
        public required string Id { get; init; }
        public required string SessionId { get; init; }
        public required ProcessPhase Phase { get; init; }
        public required ProcessType Type { get; init; }
        public required string? Target { get; init; }
        public required string? Arguments { get; init; }
        public required string? ScriptContent { get; init; }
        public required int DelayMs { get; init; }
        public required int Order { get; init; }
        public required ProcessStatus Status { get; init; }
        public required ProcessExitBehavior ExitBehavior { get; init; }
    }
}