using System.Text.Json;
using Cabby.Client.Engine.Exceptions;
using Cabby.Client.Engine.Networking.Logging;
using Cabby.Client.Shared.Contracts.Network;
using Cabby.Client.Shared.Contracts.Sessions;

namespace Cabby.Client.Engine.Commands
{
    internal sealed partial class CommandDispatcher
    {
        public async Task HandleSessionCommandAsync(WebSocketMessage<JsonElement> message)
        {
            switch (message.Type)
            {
                case WebSocketMessageTypes.Inbound.StartSession:
                {
                    var payload = message.Payload.Deserialize<StartSessionPayloadDto>(SerializerOptions)
                        ?? throw new InvalidWebSocketMessageTypeException(message.Type);

                    await sessionOrchestrator.StartNewSessionAsync(payload.Session);
                    return;
                }

                case WebSocketMessageTypes.Inbound.StopSession:
                {
                    var payload = message.Payload.Deserialize<StopSessionPayloadDto>(SerializerOptions)
                        ?? throw new InvalidWebSocketMessageTypeException(message.Type);

                    var snapshot = sessionStatusProvider.GetSnapshot();

                    if (snapshot.CurrentStatus == SessionStatus.Idle ||
                        snapshot.CurrentSession is null)
                    {
                        logger.NoSessionToStop();
                        await serverMessageDispatcher.SendAsync(WebSocketMessageTypes.Outbound.NoSessionToStop);
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(payload.Id) ||
                        payload.Id != snapshot.CurrentSession.Id)
                    {
                        logger.StopSessionIdMismatch(payload.Id, snapshot.CurrentSession.Id);
                        await serverMessageDispatcher.SendAsync(
                            WebSocketMessageTypes.Outbound.StopSessionIdMismatch,
                            new SessionIdMismatchPayloadDto
                            {
                                RequestedSessionId = payload.Id,
                                CurrentSessionId = snapshot.CurrentSession.Id
                            }
                        );
                        return;
                    };

                    sessionOrchestrator.RequestCancelSession();
                    return;
                }
            }
        }
    }
}