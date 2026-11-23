using System.Text.Json;
using System.Text.Json.Serialization;
using Cabby.Client.Engine.Commands;
using Cabby.Client.Engine.Networking.Logging;
using Cabby.Client.Shared.Contracts.Configuration;
using Cabby.Client.Shared.Contracts.Network;
using Cabby.Client.Shared.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SocketIOClient;

namespace Cabby.Client.Engine.Networking
{
    internal sealed class WebSocketConnection(
        CommandDispatcher clientCommandDispatcher,
        ILogger<WebSocketConnection> logger,
        IOptionsMonitor<CabbySettings> options) : IDisposable
    {
        private string? _serverUrl;
        private SocketIO? _server;
        private readonly SemaphoreSlim _sendLock = new(1, 1);

        public async Task SendAsync<T>(WebSocketMessage<T> message, CancellationToken ct = default)
        {
            var currentServerUrl = options.CurrentValue.ServerUrl;

            if (_server == null || _serverUrl != currentServerUrl)
            {
                await SetServerConnection(currentServerUrl);
            }

            await _sendLock.WaitAsync(ct);
            try
            {
                if (!_server!.Connected)
                {
                    logger.ConnectingToServer(_serverUrl!);
                    await _server!.ConnectAsync(ct);
                }

                logger.SendingMessage(message.ToJsonString());
                await _server!.EmitAsync(message.Type, [message], ct);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        private async Task OnServerMessageReceived(IEventContext context)
        {
            var message = context.GetValue<WebSocketMessage<JsonElement>>(0);

            if (message is null)
            {
                logger.UnreadableMessageReceived();
                return;
            }

            logger.MessageReceived(message.ToJsonString());
            await clientCommandDispatcher.DispatchAsync(message);
        }

        private async Task SetServerConnection(string? serverUrl)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                logger.ServerUrlNotConfigured();
                throw new InvalidOperationException("Server URL is not configured.");
            }

            _server?.Dispose();

            logger.SettingServerConnection(serverUrl);
            _serverUrl = serverUrl;
            _server = new SocketIO(new Uri(_serverUrl), new SocketIOOptions
            {
                EIO = SocketIOClient.Common.EngineIO.V4,
                Transport = SocketIOClient.Common.TransportProtocol.WebSocket,
                Reconnection = true,
                ReconnectionAttempts = int.MaxValue
            },
            services =>
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper) },
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = false
                };

                services.AddSystemTextJson(jsonOptions);
            });

            _server.On("message", OnServerMessageReceived);
        }

        public void Dispose()
        {
            _server?.Dispose();
            _sendLock.Dispose();
        }
    }
}