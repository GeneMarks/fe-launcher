using Microsoft.Extensions.Logging;

namespace Cabby.Client.Engine.Networking.Logging
{
    internal static partial class CommandDispatcherSessionsLogging
    {
        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Server requested to stop session when there is no current session.")]
        public static partial void NoSessionToStop(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Session ID ({RequestedSessionId}) requested to be stopped by server does not match current Session ID ({CurrentSessionId}).")]
        public static partial void StopSessionIdMismatch(this ILogger logger, string? requestedSessionId, string? currentSessionId);
    }
}