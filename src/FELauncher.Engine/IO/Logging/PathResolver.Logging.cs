using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.IO.Logging
{
    internal static partial class PathResolverLogging
    {
        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "Provided path is null or empty. Returning empty path.")]
        public static partial void PathNullOrEmpty(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Provided path is already absolute. Returning path '{Path}'.")]
        public static partial void PathAbsolute(this ILogger logger, string path);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Resolved path to '{ResolvedPath}'.")]
        public static partial void ResolvedResult(this ILogger logger, string resolvedPath);
    }
}

