namespace Cabby.Client.Shared.Contracts.Sessions
{
    public enum SessionStatus
    {
        Idle,
        Pending,
        RunningPreProcesses,
        RunningFrontend,
        RunningPostProcesses,
        Stopping,
        Completed,
        Aborted,
        Failed
    }
}