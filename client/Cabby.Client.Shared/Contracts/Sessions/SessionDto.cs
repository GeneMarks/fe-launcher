using Cabby.Client.Shared.Contracts.Processes;

namespace Cabby.Client.Shared.Contracts.Sessions
{
    public sealed record SessionDto
    {
        public required string Id { get; init; }
        public required string ClientId { get; init; }
        public required int GracefulCloseWindowsTimeoutMs { get; init; }
        public required SessionStatus Status { get; init; }
        public required ProcessDto[] PreProcesses { get; init; }
        public required ProcessDto MainProcess { get; init; }
        public required ProcessDto[] PostProcesses { get; init; }
    }
}