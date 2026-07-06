using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Sessions.Logging
{
    internal static partial class StartupSessionInitializerLogging
    {
        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Automatic session launch enabled. New session will start momentarily.")]
        public static partial void StartupSessionEnabled(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Automatic session launch disabled.")]
        public static partial void StartupSessionDisabled(this ILogger logger);
    }
}
