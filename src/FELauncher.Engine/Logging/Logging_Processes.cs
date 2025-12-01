using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Logging
{
    public static partial class EngineLog
    {
        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "The process path '{filePath}' is invalid or empty.")]
        public static partial void Processes_InvalidPath(this ILogger logger, string filePath);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "The file '{FilePath}' contains an invalid extension.\nRequired file extension: {RequiredExtension}")]
        public static partial void Processes_InvalidFileExt(this ILogger logger, string filePath, string requiredExtension);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "The file '{FilePath}' does not exist.")]
        public static partial void Processes_FileNotPresent(this ILogger logger, string filePath);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "The file '{FilePath}' is not an executable.")]
        public static partial void Processes_FileNotExecutable(this ILogger logger, string filePath);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "The process '{ProcessFile}' failed to start for an unknown reason.")]
        public static partial void Processes_FailedToStartUnknown(this ILogger logger, string processFile);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "The process '{ProcessFile}' failed to start: {Message}")]
        public static partial void Processes_FailedToStart(this ILogger logger, string processFile, string message);
    }
}
