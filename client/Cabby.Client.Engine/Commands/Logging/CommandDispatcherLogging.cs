using Microsoft.Extensions.Logging;

namespace Cabby.Client.Engine.Networking.Logging
{
    internal static partial class CommandDispatcherLogging
    {
        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Recieved unknown message type from server: '{MessageType}'.")]
        public static partial void ReceivedUnknownMessageType(this ILogger logger, string? messageType);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Recieved message from server of type: '{MessageType}'.")]
        public static partial void ReceivedValidMessageType(this ILogger logger, string? messageType);
    }
}