namespace Cabby.Client.Shared.Contracts.Sessions
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
