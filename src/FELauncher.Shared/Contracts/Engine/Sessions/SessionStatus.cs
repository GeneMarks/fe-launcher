namespace FELauncher.Shared.Contracts.Engine.Sessions
{
    public enum SessionStatus
    {
        Created,
        Starting,
        RunningPreHooks,
        RunningPreProcesses,
        RunningFrontend,
        RunningPostHooks,
        Stopping,
        Completed,
        Aborted,
        Failed
    }
}
