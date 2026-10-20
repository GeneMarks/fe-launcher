namespace Cabby.Client.Shared.Contracts.TaskScheduling
{
    public interface IStartupTaskScheduler
    {
        TaskRegisterResult RegisterTaskIfNotExists(string taskName);
        TaskDeleteResult EnsureTaskIsDeleted(string taskName);
    }
}
