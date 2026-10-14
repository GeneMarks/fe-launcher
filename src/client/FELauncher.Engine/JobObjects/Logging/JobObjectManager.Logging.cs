using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.JobObjects.Logging
{
    internal static partial class JobObjectManagerLogging
    {
        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to create job object. Win32Error: {Win32ErrorCode}")]
        public static partial void FailedToCreateJobObject(this ILogger logger, int win32ErrorCode, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to terminate job object. Win32Error: {Win32ErrorCode}")]
        public static partial void FailedToTerminateJobObject(this ILogger logger, int win32ErrorCode, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to create IO completion port. Win32Error: {Win32ErrorCode}")]
        public static partial void FailedToCreateIoCompletionPort(this ILogger logger, int win32ErrorCode, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to set job object limits. Win32Error: {Win32ErrorCode}")]
        public static partial void FailedToSetJobObjectLimits(this ILogger logger, int win32ErrorCode, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to query job object information. Skipping wait loop. Win32Error: {Win32ErrorCode}")]
        public static partial void FailedToQueryJobObjectInfoInWait(this ILogger logger, int win32ErrorCode, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to get queued completion status. Ending wait loop. Win32Error: {Win32ErrorCode}")]
        public static partial void FailedToGetCompletionStatusInWait(this ILogger logger, int win32ErrorCode, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Tried to access null or invalid job handle.")]
        public static partial void TriedToAccessNullOrInvalidJobHandle(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Could not determine if pid {ProcessId} is in current job. Win32Error: {Win32ErrorCode}")]
        public static partial void CouldNotDetermineIfProcInJob(this ILogger logger, uint processId, int win32ErrorCode, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to post message to window handle belonging to pid {ProcessId}. Win32Error: {Win32ErrorCode}")]
        public static partial void FailedToPostMessage(this ILogger logger, uint processId, int win32ErrorCode, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Tried to wait for completion of null or invalid job handle.")]
        public static partial void TriedToWaitNullOrInvalidJobHandle(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Job handle is null or invalid. Aborting window enumeration.")]
        public static partial void CannotAccessJobInWindowEnum(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Waiting grace period of {GracePeriodSeconds} seconds to allow window handles to close.")]
        public static partial void WaitingGracePeriod(this ILogger logger, int gracePeriodSeconds);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Job object does not contain any active processes. Skipping wait loop.")]
        public static partial void NoActiveProcsInJobInWait(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Skipping termination. Job object is already null or invalid.")]
        public static partial void TerminationUnnecessary(this ILogger logger);
    }
}
