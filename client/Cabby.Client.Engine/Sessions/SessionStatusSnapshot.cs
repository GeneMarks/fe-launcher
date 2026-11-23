using Cabby.Client.Shared.Contracts.Sessions;

namespace Cabby.Client.Engine.Sessions
{
    public sealed record SessionStatusSnapshot
    {
        public required SessionStatus CurrentStatus { get; init; }
        public required SessionDto? CurrentSession { get; init; }
        public required bool IsIdle { get; init; }
        public required bool IsActive { get; init; }
        public required bool IsCancellable { get; init; }
        public required TimeSpan ElapsedTime { get; init; }
    }
}