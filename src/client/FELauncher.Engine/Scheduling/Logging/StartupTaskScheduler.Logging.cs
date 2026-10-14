using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Scheduling.Logging
{
    internal static partial class StartupTaskSchedulerLogging
    {
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
            Message = "Startup task '{TaskName}' already exists. Skipping registration.")]
        public static partial void StartupTaskAlreadyExists(this ILogger logger, string taskName);
    }
}
