using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Sessions
{
    internal static partial class JobObjectManagerLogging
    {
        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to create job object. Win32Error: {Win32ErrorCode} ({Win32Message})")]
        public static partial void FailedToCreateJobObject(this ILogger logger, int win32ErrorCode, string win32Message);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to terminate job object. Win32Error: {Win32ErrorCode} ({Win32Message})")]
        public static partial void FailedToTerminateJobObject(this ILogger logger, int win32ErrorCode, string win32Message);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to create IO completion port. Win32Error: {Win32ErrorCode} ({Win32Message})")]
        public static partial void FailedToCreateIoCompletionPort(this ILogger logger, int win32ErrorCode, string win32Message);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to set job object limits. Win32Error: {Win32ErrorCode} ({Win32Message})")]
        public static partial void FailedToSetJobObjectLimits(this ILogger logger, int win32ErrorCode, string win32Message);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Tried to access null or invalid job handle.")]
        public static partial void TriedToAccessNullOrInvalidJobHandle(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Tried to wait for completion of null or invalid job handle.")]
        public static partial void TriedToWaitNullOrInvalidJobHandle(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Skipping termination. Job object is already null or invalid.")]
        public static partial void TerminationUnecessary(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Failed to get queued completion status. Ending wait loop.")]
        public static partial void FailedToGetCompletionStatus(this ILogger logger);
    }
}
