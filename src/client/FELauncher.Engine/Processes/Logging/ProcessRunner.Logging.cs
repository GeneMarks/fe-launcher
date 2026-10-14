using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes.Logging
{
    internal static partial class ProcessRunnerLogging
    {
        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "PID {ProcessId} ({ProcessPath}) exited with exit code: {ExitCode}.")]
        public static partial void ProcessExited(this ILogger logger, uint processId, string processPath, uint exitCode);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Starting process '{ProcessName}' ({ProcessPathWithArgs}).")]
        public static partial void StartingProcess(this ILogger logger, string processName, string processPathWithArgs);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Waiting delay of {DelaySeconds} seconds before starting process '{ProcessName}' ({ProcessPathWithArgs}).")]
        public static partial void WaitingDelay(this ILogger logger, int delaySeconds, string processName, string processPathWithArgs);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "No processes to run in the provided ProcessRun.")]
        public static partial void NoProcessesInProcessRun(this ILogger logger);
    }
}

