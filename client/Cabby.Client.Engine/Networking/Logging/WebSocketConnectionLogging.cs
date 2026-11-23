using Microsoft.Extensions.Logging;

namespace Cabby.Client.Engine.Networking.Logging
{
    internal static partial class WebSocketConnectionLogging
    {
        [LoggerMessage(
            Level = LogLevel.Error,
            Message = "Server URL is not configured.")]
        public static partial void ServerUrlNotConfigured(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Warning,
            Message = "Unreadable message recieved from server.")]
        public static partial void UnreadableMessageReceived(this ILogger logger);

        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "Establishing connection to server '{ServerUrl}'.")]
        public static partial void ConnectingToServer(this ILogger logger, string serverUrl);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Sending message to server:\n{Message}")]
        public static partial void SendingMessage(this ILogger logger, string message);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Setting new server connection to '{ServerUrl}'.")]
        public static partial void SettingServerConnection(this ILogger logger, string serverUrl);

        [LoggerMessage(
            Level = LogLevel.Debug,
            Message = "Message recieved from server:\n{Message}")]
        public static partial void MessageReceived(this ILogger logger, string message);
    }
}