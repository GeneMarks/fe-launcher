using Microsoft.Extensions.Logging;

namespace Cabby.Client.Engine.TaskScheduling.Logging
{
    internal static partial class StartupTaskSchedulerLogging
    {
        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to register task '{TaskName}'.")]
        public static partial void FailedToRegisterTask(this ILogger logger, string taskName);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to delete task '{TaskName}'.")]
        public static partial void FailedToDeleteTask(this ILogger logger, string taskName);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Registering task '{TaskName}' in Task Scheduler.")]
        public static partial void RegisteringTask(this ILogger logger, string taskName);
        
        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Ensuring task '{TaskName}' is deleted from Task Scheduler.")]
        public static partial void EnsuringTaskIsDeleted(this ILogger logger, string taskName);
        
        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Task '{TaskName}' already exists. Skipping registration.")]
        public static partial void TaskAlreadyExists(this ILogger logger, string taskName);
    }
}
