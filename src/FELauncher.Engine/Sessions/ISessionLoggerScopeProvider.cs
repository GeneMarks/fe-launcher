using Microsoft.Extensions.Logging;

namespace FELauncher.Engine.Sessions
{
    public interface ISessionLoggerScopeProvider
    {
        /// <summary>
        /// Updates the current session id used for subsequently created scopes.
        /// Pass <c>null</c> to clear.
        /// </summary>
        void SetCurrentSessionId(string? sessionId);

        /// <summary>
        /// Begins a logging scope that includes the current session id (if set).
        /// Returns <c>null</c> when no session is active.
        /// </summary>
        IDisposable? BeginSessionScope(ILogger logger);
    }
}
