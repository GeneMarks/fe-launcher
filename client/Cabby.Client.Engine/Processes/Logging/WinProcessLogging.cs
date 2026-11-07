using Microsoft.Extensions.Logging;

namespace Cabby.Client.Engine.Processes.Logging
{
    internal static partial class WinProcessLogging
    {
        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to initialize process attribute list for process with path '{ProcessPath}'. Win32Error: {Win32ErrorCode}")]
        public static partial void FailedToInitializeAttributeList(this ILogger logger, string processPath, int win32ErrorCode, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to update process attribute list for process with path '{ProcessPath}'. Win32Error: {Win32ErrorCode}")]
        public static partial void FailedToUpdateAttributeList(this ILogger logger, string processPath, int win32ErrorCode, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to create process with path '{ProcessPath}'. Win32Error: {Win32ErrorCode}")]
        public static partial void FailedToCreateProcess(this ILogger logger, string processPath, int win32ErrorCode, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Process {PID} '{PrettyName}' ({ProcessPath}) exited with code: {ExitCode}")]
        public static partial void ProcessExited(this ILogger logger, uint pid, string processPath, string prettyName, uint exitCode);
    }
}
