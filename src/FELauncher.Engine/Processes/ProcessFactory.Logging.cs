using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Processes
{
    internal static partial class ProcessFactoryLogging
    {
        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "The provided executable path is empty.")]
        public static partial void EmptyPath(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "The file '{FilePath}' is not an executable.\nRequired file type: {RequiredExtension}")]
        public static partial void InvalidFileExt(this ILogger logger, string filePath, string requiredExtension);

        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "The file '{FilePath}' does not exist.")]
        public static partial void FileNotPresent(this ILogger logger, string filePath);
    }
}
