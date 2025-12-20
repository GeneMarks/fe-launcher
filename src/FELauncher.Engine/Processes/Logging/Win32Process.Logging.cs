using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes.Logging
{
    internal static partial class Win32ProcessLogging
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
            Message = "Failed to register wait operation for pid {ProcessId} ({ProcessPath}). Win32Error: {Win32ErrorCode}")]
        public static partial void FailedToRegisterWaitOperation(this ILogger logger, uint processId, string processPath, int win32ErrorCode, Exception ex);
    }
}
