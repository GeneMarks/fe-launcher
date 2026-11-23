using Cabby.Client.Shared.Contracts.Network;

namespace Cabby.Client.Engine.Networking
{
    internal sealed class ServerMessageDispatcher(WebSocketConnection webSocketConnection)
    {
        public async Task SendAsync(string type, CancellationToken ct = default)
        {
            var message = new WebSocketMessage<object?>
            {
                Type = type,
                Payload = null
            };

            await webSocketConnection.SendAsync(message, ct);
        }

        public async Task SendAsync<T>(string type, T payload, CancellationToken ct = default)
        {
            var message = new WebSocketMessage<T>
            {
                Type = type,
                Payload = payload
            };

            await webSocketConnection.SendAsync(message, ct);
        }
    }
}