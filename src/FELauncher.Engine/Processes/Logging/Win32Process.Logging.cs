using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes.Logging
{
    internal static partial class Win32ProcessLogging
    {
        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to initialize process attribute list for process '{Process}'. Win32Error: {Win32ErrorCode} ({Win32Message})")]
        public static partial void FailedToInitializeAttributeList(this ILogger logger, string process, int win32ErrorCode, string win32Message);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to update process attribute list for process '{Process}'. Win32Error: {Win32ErrorCode} ({Win32Message})")]
        public static partial void FailedToUpdateAttributeList(this ILogger logger, string process, int win32ErrorCode, string win32Message);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to create process '{Process}'. Win32Error: {Win32ErrorCode} ({Win32Message})")]
        public static partial void FailedToCreateProcess(this ILogger logger, string process, int win32ErrorCode, string win32Message);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to register wait operation for process '{Process}'. Win32Error: {Win32ErrorCode} ({Win32Message})")]
        public static partial void FailedToRegisterWaitOperation(this ILogger logger, string process, int win32ErrorCode, string win32Message);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Process '{Process}' exited with the exit code: {ExitCode}.)")]
        public static partial void ProcessExited(this ILogger logger, string process, uint exitCode);
    }
}
