namespace Cabby.Client.Engine.TaskScheduling
{
    public record TaskRegisterResult(
        bool Success,
        string TaskName,
        string Message,
        bool TaskAlreadyExists
    );
}
