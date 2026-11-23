using System.Text.Json;
using Cabby.Client.Engine.Networking;
using Cabby.Client.Engine.Networking.Logging;
using Cabby.Client.Engine.Sessions;
using Cabby.Client.Shared.Contracts.Network;
using Microsoft.Extensions.Logging;

namespace Cabby.Client.Engine.Commands
{
    internal sealed partial class CommandDispatcher(
        SessionOrchestrator sessionOrchestrator,
        SessionStatusProvider sessionStatusProvider,
        ServerMessageDispatcher serverMessageDispatcher,
        ILogger<CommandDispatcher> logger
    )
    {
        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

        public async Task DispatchAsync(WebSocketMessage<JsonElement> message)
        {
            switch (message.Type)
            {
                case WebSocketMessageTypes.Inbound.StartSession:
                case WebSocketMessageTypes.Inbound.StopSession:
                    logger.ReceivedValidMessageType(message.Type);
                    await HandleSessionCommandAsync(message);
                    break;

                case WebSocketMessageTypes.Inbound.StartProcess:
                case WebSocketMessageTypes.Inbound.StopProcess:
                    logger.ReceivedValidMessageType(message.Type);
                    // await HandleProcessCommandAsync(message);
                    break;

                default:
                    logger.ReceivedUnknownMessageType(message.Type);
                    await serverMessageDispatcher.SendAsync(WebSocketMessageTypes.Outbound.UnknownMessageType);
                    break;
            }
        }
    }
}