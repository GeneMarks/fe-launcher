using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes.Logging
{
    internal static partial class FELProcessManagerLogging
    {
        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Pid {ProcessId} ({ProcessPath}) exited with exit code: {ExitCode}.")]
        public static partial void Win32ProcessExited(this ILogger logger, uint processId, string processPath, uint exitCode);
    }
}

