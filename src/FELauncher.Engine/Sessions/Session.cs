namespace FELauncher.Engine.Sessions
{
    public enum SessionStatus
    {
        Created,
        Starting,
        RunningPreHooks,
        RunningPreProcesses,
        RunningFrontend,
        RunningPostHooks,
        Completed,
        Aborted,
        Failed
    }

    public sealed class Session
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
        public SessionStatus Status { get; internal set; } = SessionStatus.Created;
    }
}
