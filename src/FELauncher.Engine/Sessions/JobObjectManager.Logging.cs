using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FELauncher.Engine.Sessions
{
    internal static partial class JobObjectManagerLogging
    {
        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Job object creation failed with the error: {Win32Message}")]
        public static partial void FailedToCreateJobObject(this ILogger logger, string win32Message);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to add process '{Process}' to job object with the error: {Win32Message}")]
        public static partial void FailedToAssignProcessToJobObject(this ILogger logger, Process process, string win32Message);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Failed to terminate job object with the error: {Win32Message}")]
        public static partial void FailedToTerminateJobObject(this ILogger logger, string win32Message);
    }
}
