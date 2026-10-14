using FELauncher.Shared.Contracts.Sessions;
using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Sessions
{
    internal sealed class SessionLoggerScopeProvider : ISessionLoggerScopeProvider
    {
        private readonly Lock _lock = new();
        private string? _currentSessionId;

        public void SetCurrentSessionId(string? sessionId)
        {
            lock (_lock)
            {
                _currentSessionId = sessionId;
            }
        }

        public IDisposable? BeginSessionScope(ILogger logger)
        {
            string? sessionId;

            lock (_lock)
            {
                sessionId = _currentSessionId;
            }

            return sessionId is null
                ? null
                : logger.BeginScope(new Dictionary<string, object>
                {
                    ["SessionId"] = sessionId
                });
        }


    }
}
