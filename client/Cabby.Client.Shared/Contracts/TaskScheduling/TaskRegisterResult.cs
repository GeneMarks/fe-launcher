namespace Cabby.Client.Shared.Contracts.TaskScheduling
{
    public record TaskRegisterResult(
        bool Success,
        string TaskName,
        string Message,
        bool TaskAlreadyExists
    );
}
