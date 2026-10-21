namespace Cabby.Client.Engine.TaskScheduling
{
    public record TaskDeleteResult(
        bool Success,
        string TaskName,
        string Message
    );
}
