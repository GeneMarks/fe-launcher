namespace FELauncher.Shared.Contracts.Sessions
{
    public interface ISessionOrchestrator
    {
        /// <summary>
        /// Creates and runs a new session if an active one isn't already being orchestrated.
        /// </summary>
        Task StartNewSessionAsync();

        /// <summary>
        /// Requests cancellation from the cancellation token source if the session is in a cancelable state.
        /// </summary>
        void RequestEndSession();

        /// <summary>
        /// Returns an immutable snapshot of the current session state.
        /// </summary>
        SessionStateSnapshot GetSessionState();
    }
}
