using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Logging
{
    public static partial class EngineLog
    {
        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "Provided path is null or empty. Returning empty path.")]
        public static partial void IO_ResolverPathNullOrEmpty(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Provided path is already absolute. Returning path '{Path}'.")]
        public static partial void IO_ResolverPathAbsolute(this ILogger logger, string path);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Resolved path to '{ResolvedPath}'.")]
        public static partial void IO_ResolverResult(this ILogger logger, string resolvedPath);
    }
}

