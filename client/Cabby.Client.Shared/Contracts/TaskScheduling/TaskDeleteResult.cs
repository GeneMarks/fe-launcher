namespace Cabby.Client.Shared.Contracts.TaskScheduling
{
    public record TaskDeleteResult(
        bool Success,
        string TaskName,
        string Message
    );
}
